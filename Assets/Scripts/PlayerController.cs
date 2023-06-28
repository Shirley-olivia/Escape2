using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;


public class PlayerController : MonoBehaviour
{
    private PlayerInputAction inputAction;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction fireAction;
    private InputAction viewAction;
    [Header("Movement")]
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 5.0f;
    public float gravity = 9.8f;
    public Transform followTarget;
    public Transform flashLight;
    private CharacterController characterController;
    private bool isRunning;
    [Header("Rotation")]
    public Transform jammo;
    public Transform headTargetWrapper;
    public float rotateSensitivity = 0.1f;
    public float rotateEpsinon = 1f;
    public float rotateAngle;
    [Header("Firing")]
    public MuzzleController muzzle;
    public GameObject moveCamera;
    public GameObject aimCamera;
    public GameObject aiming;
    private bool isFiring;
    [Header("Finger")]
    public FingerController fingerController;
    private bool leftShrink;
    private bool rightShrink;
    [Header("Animator")]
    public Animator animator;
    public bool gameOver;
    [Header("Health")]
    public int maxHealth = 20;
    public int health { get { return currentHealth; } }
    public int currentHealth;
    public TextMeshProUGUI blood;
    [Header("View")]
    public GameObject cameraThird;
    public GameObject cameraFirst;
    [Header("Grab")]
    public GameObject DoorKey;
    public float grabSpeed = 0.1f;
    public IKSolver RightHand;
    public GameObject RightArmTarget;
    private bool Door = false;
    public GameObject[] fingers;
    public LayerMask objectLayer;
    public GameObject rightHand;
    public GameObject rightThumb;
    public float rotationAngle = 1f;
    [Header("Game Over")]
    public GameObject End;
    public PostProcessVolume postProcessVolume;

    void Awake()
    {
        inputAction = new PlayerInputAction();
    }

    void OnEnable()
    {
        inputAction.Enable();
        moveAction = inputAction.Player.Move;
        lookAction = inputAction.Player.Look;
        fireAction = inputAction.Player.Fire;
        fireAction.performed += FireStart;
        fireAction.canceled += FireCancel;
        viewAction = inputAction.Player.View;
        viewAction.performed += ChangeView;
    }

    void Start()
    {
        rotateAngle = 0f;
        isFiring = false;
        leftShrink = false;
        rightShrink = false;
        gameOver = false;
        currentHealth = maxHealth;
        characterController = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        blood.text = currentHealth.ToString();

        Move();
        RotateView();
        ChangeFingers();
        isDie();
        
        PostProcessingDistance();
    }

    void LateUpdate()
    {
        RotateForward();
    }

    void Move()
    {
        Vector2 move = moveAction.ReadValue<Vector2>();
        float moveX = move.x;
        float moveY = -gravity;
        float moveZ = move.y;
        Vector3 speed = moveX * transform.right + moveY * transform.up + moveZ * transform.forward;
        if (isFiring)
        {
            speed *= 0.5f;
        }
        isRunning = moveX != 0f || moveZ != 0f;
        animator.SetBool("isRunning", isRunning);
        characterController.Move(speed * moveSpeed * Time.deltaTime);
    }

    void RotateView()
    {
        Vector2 look = lookAction.ReadValue<Vector2>();
        float rotateX = look.x;
        float rotateY = look.y;
        if (isFiring)
        {
            rotateX *= 0.5f;
            rotateY *= 0.5f;
        }
        followTarget.rotation *= Quaternion.AngleAxis(rotateX * rotateSpeed, Vector3.up);
        followTarget.rotation *= Quaternion.AngleAxis(rotateY * rotateSpeed, Vector3.right);
        flashLight.rotation *= Quaternion.AngleAxis(rotateX * rotateSpeed, Vector3.up);
        flashLight.rotation *= Quaternion.AngleAxis(rotateY * rotateSpeed, Vector3.right);
        Vector3 angles = followTarget.localEulerAngles;
        float angle = angles.x;
        angles.z = 0;
        if (angle > 180 && angle < 280)
        {
            angles.x = 280;
        }
        else if (angle < 180 && angle > 80)
        {
            angles.x = 80;
        }
        followTarget.localEulerAngles = angles;
        flashLight.localEulerAngles = angles;
        transform.rotation = Quaternion.Euler(0, followTarget.rotation.eulerAngles.y, 0);
        followTarget.localEulerAngles = new Vector3(angles.x, 0, 0);
        flashLight.localEulerAngles = new Vector3(angles.x, 0, 0);
    }

    void RotateForward()
    {
        Vector2 move = moveAction.ReadValue<Vector2>();
        float moveX = move.x;
        float moveZ = move.y;
        if (moveZ > 0 && moveX > 0)
        {
            if (rotateAngle != 30f)
                rotateAngle = 30f;
        }
        else if (moveZ > 0 && moveX < 0)
        {
            if (rotateAngle != -30f)
                rotateAngle = -30f;
        }
        else if (moveZ == 0 && moveX > 0)
        {
            if (rotateAngle != 60f)
                rotateAngle = 60f;
        }
        else if (moveZ == 0 && moveX < 0)
        {
            if (rotateAngle != -60f)
                rotateAngle = -60f;
        }
        else if (rotateAngle != 0f)
            rotateAngle = 0f;
        if (jammo.localRotation.eulerAngles.y != rotateAngle)
        {
            float deltaAngle = jammo.localRotation.eulerAngles.y - rotateAngle;
            if (deltaAngle < -180)
            {
                deltaAngle += 360;
            }
            else if (deltaAngle > 180)
            {
                deltaAngle -= 360;
            }
            if (deltaAngle < -rotateEpsinon)
            {
                jammo.Rotate(0, Time.deltaTime * rotateSensitivity, 0);
                headTargetWrapper.Rotate(0, -Time.deltaTime * rotateSensitivity, 0);
            }
            else if (deltaAngle > rotateEpsinon)
            {
                jammo.Rotate(0, -Time.deltaTime * rotateSensitivity, 0);
                headTargetWrapper.Rotate(0, Time.deltaTime * rotateSensitivity, 0);
            }
        }
    }

    private void FireStart(InputAction.CallbackContext context)
    {
        isFiring = true;
        animator.SetBool("isFiring", isFiring);
        if (!aimCamera.activeInHierarchy)
        {
            moveCamera.SetActive(false);
            aimCamera.SetActive(true);
            aiming.SetActive(true);
        }
        muzzle.isFiring = true;
    }

    private void FireCancel(InputAction.CallbackContext context)
    {
        isFiring = false;
        animator.SetBool("isFiring", isFiring);
        if (!moveCamera.activeInHierarchy)
        {
            moveCamera.SetActive(true);
            aimCamera.SetActive(false);
            aiming.SetActive(false);
        }
        muzzle.isFiring = false;
    }

    private void ChangeView(InputAction.CallbackContext context)
    {
        if (cameraThird.activeInHierarchy)
        {
            cameraFirst.SetActive(true);
            cameraThird.SetActive(false);
        }
        else
        {
            cameraFirst.SetActive(false);
            cameraThird.SetActive(true);
        }
    }

    void ChangeFingers()
    {
        if (isFiring)
        {
            if (leftShrink)
            {
                leftShrink = false;
                fingerController.Expand(FingerController.Hand.Left);
            }
            if (!rightShrink)
            {
                rightShrink = true;
                fingerController.Shrink(FingerController.Hand.Right);
            }
        }
        else if (isRunning)
        {
            if (!leftShrink)
            {
                leftShrink = true;
                fingerController.Shrink(FingerController.Hand.Left);
            }
            if (!rightShrink)
            {
                rightShrink = true;
                fingerController.Shrink(FingerController.Hand.Right);
            }
        }
        else
        {
            if (leftShrink)
            {
                leftShrink = false;
                fingerController.Expand(FingerController.Hand.Left);
            }
            if (rightShrink)
            {
                rightShrink = false;
                fingerController.Expand(FingerController.Hand.Right);
            }
        }
    }

    public void Grab(GameObject grabObject, bool isGrab)
    {
        DoorKey = grabObject;
        float distance = Vector3.Distance(RightArmTarget.transform.position, DoorKey.transform.position);
        if (isGrab)
        {
            Door = true;
            //animator.SetBool("isGrab", isGrab);
            //PauseAllAnimations();
            animator.enabled = false;

            Vector3 sphereCenter = DoorKey.transform.position;
            float sphereRadius = DoorKey.GetComponent<SphereCollider>().radius;
            Debug.Log(sphereRadius);
            //Debug.Log(distance);
            while (distance > (sphereRadius - 0.25f))
            {
                //Debug.Log(distance);
                Vector3 direction = (DoorKey.transform.position - RightArmTarget.transform.position).normalized;
                //print("direction: " + direction);
                Vector3 perpendicular = Vector3.Cross(direction, Vector3.right).normalized;
                //print("perpendicular: " + perpendicular);
                Quaternion targetRotation = Quaternion.LookRotation(direction, -perpendicular);
                //print("targetRotation: " + targetRotation);
                RightArmTarget.transform.position += direction * grabSpeed * Time.deltaTime;
                //Debug.Log(RightArmTarget.transform.position);
                Quaternion adjustedRotation = Quaternion.Euler(40f, 0f, 0f);
                RightArmTarget.transform.rotation = targetRotation * adjustedRotation;
                distance = Vector3.Distance(RightArmTarget.transform.position, DoorKey.transform.position);
                //Debug.Log(distance);
            }
            //Vector3 fingerPosition = fingers[0].transform.position;
            //float distanceToSurface = Vector3.Distance(fingerPosition, sphereCenter);
            //while (distanceToSurface > sphereRadius)
            //{
            //    Vector3 axisPositionR = rightHand.transform.position;
            //    Vector3 axisDirectionR = rightHand.transform.right;
            //    RightArmTarget.transform.Rotate(axisDirectionR, rotationAngle);
            //    distanceToSurface = Vector3.Distance(fingers[0].transform.position, sphereCenter);
            //}

            // for (int i = 0; i < 3; i++)
            // {
            //     Debug.Log(i);
            //     Vector3 fingerPosition = fingers[i].transform.position;
            //     float distanceToSurface = Vector3.Distance(fingerPosition, sphereCenter);
            //     Debug.Log(distanceToSurface);

            //     while (distanceToSurface > (sphereRadius + 0.35f))
            //     {
            //         Vector3 axisPositionR = rightHand.transform.position;
            //         Vector3 axisDirectionR = rightHand.transform.right;
            //         fingers[i].transform.RotateAround(axisPositionR, axisDirectionR, rotationAngle);
            //         distanceToSurface = Vector3.Distance(fingers[i].transform.position, sphereCenter);
            //         //Debug.Log(distanceToSurface);
            //     }

            //     for (int j = 0; j < 8; j++)
            //     {
            //         Vector3 axisPositionR = rightHand.transform.position;
            //         Vector3 axisDirectionR = rightHand.transform.right;
            //         fingers[i].transform.RotateAround(axisPositionR, axisDirectionR, rotationAngle);
            //         distanceToSurface = Vector3.Distance(fingers[i].transform.position, sphereCenter);
            //         Debug.Log(distanceToSurface);
            //     }
            // }

            // Vector3 fingerPositionT = fingers[3].transform.position;
            // float distanceToSurfaceT = Vector3.Distance(fingerPositionT, sphereCenter);
            // Debug.Log(distanceToSurfaceT);
            // while (distanceToSurfaceT > (sphereRadius + 0.35f))
            // {
            //     Vector3 axisPositionRT = rightThumb.transform.position;
            //     Vector3 axisDirectionRT = rightThumb.transform.up;
            //     fingers[3].transform.RotateAround(axisPositionRT, axisDirectionRT, rotationAngle);
            //     distanceToSurfaceT = Vector3.Distance(fingers[3].transform.position, sphereCenter);
            // }
            DoorKey.transform.SetParent(RightArmTarget.transform);
            animator.enabled = true;

            //RightArmTarget.transform.position += new Vector3(0f, 0.3f, 0f);
        }
        else
        {
            Door = false;
            DoorKey.transform.SetParent(null);
            DoorKey = null;
            //animator.SetBool("isGrab", isGrab);
        }
    }

    public void ChangeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        //Debug.Log(currentHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    public void GameOver(bool win)
    {
        gameOver = true;
        inputAction.Disable();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameManager.GameOver(win);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == End)
        {
            GameOver(true);
        }
        else if (other.CompareTag("EnemyActiveTrigger"))
        {
            EnemyActiveTrigger trigger = other.GetComponent<EnemyActiveTrigger>();
            trigger.SetEnemyActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EnemyActiveTrigger"))
        {
            EnemyActiveTrigger trigger = other.GetComponent<EnemyActiveTrigger>();
            trigger.SetEnemyActive(false);
        }
    }

    public void isDie()
    {
        if(currentHealth <= 0)
        {
            GameOver(false);
        }
    }

    private void PostProcessingDistance()
    {
        float distance = Vector3.Distance(transform.position, End.transform.position);
        if (distance < 20)
        {
            postProcessVolume.weight = 1 - (distance / 20);
        }
        else
        {
            postProcessVolume.weight = 0;
        }
    }

}
