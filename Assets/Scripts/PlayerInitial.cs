using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInitial : MonoBehaviour
{
    private PlayerInputAction inputAction;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    [Header("Movement")]
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 5.0f;
    public Transform followTarget;
    private bool isRunning;
    [Header("Rotation")]
    public Transform jammo;
    public Transform headTargetWrapper;
    public float rotateSensitivity = 0.1f;
    public float rotateEpsinon = 1f;
    public float rotateAngle;
    [Header("Finger")]
    public FingerController fingerController;
    private bool leftShrink;
    private bool rightShrink;
    [Header("Animator")]
    public Animator animator;
    public bool gameOver;

    public int maxHealth = 20;
    public int health { get { return currentHealth; } }
    public int currentHealth;

    public GameObject camera01;
    public GameObject hip;
    public Rigidbody target_right_foot;
    public Rigidbody pole_right_foot;
    public Rigidbody target_left_foot;
    public Rigidbody pole_left_foot;
    public Rigidbody RightArmTarget;
    public Rigidbody RightArmPole;
    public Rigidbody LeftArmTarget;
    public Rigidbody LeftArmPole;
    public Rigidbody HeadTarget;
    public Rigidbody HeadPole;

    public GameObject EASY;
    public GameObject NORMAL;
    public GameObject HARD;
    public GameManager gameManager;

    void Awake()
    {
        inputAction = new PlayerInputAction();
    }

    void OnEnable()
    {
        inputAction.Enable();
        moveAction = inputAction.Player.Move;
        lookAction = inputAction.Player.Look;
        jumpAction = inputAction.Player.Jump;
        jumpAction.performed += Jump;
    }

    void Start()
    {
        rotateAngle = 0f;
        leftShrink = false;
        rightShrink = false;
        gameOver = false;
        currentHealth = maxHealth;
        //characterController = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Move();
        ChangeFingers();
        check();
    }

    void LateUpdate()
    {
        RotateForward();
    }

    void Move()
    {
        Vector2 move = moveAction.ReadValue<Vector2>();
        float moveX = move.x;
        float moveZ = move.y;
        Vector3 speed = new Vector3(-moveX, 0, -moveZ);
        isRunning = moveX != 0f || moveZ != 0f;
        Rigidbody rbhip = hip.GetComponent<Rigidbody>();
        rbhip.AddForce(-speed * 5f);
    }

    void RotateView()
    {
        Vector2 look = lookAction.ReadValue<Vector2>();
        float rotateX = look.x;
        float rotateY = look.y;
        followTarget.rotation *= Quaternion.AngleAxis(rotateX * rotateSpeed, Vector3.up);
        followTarget.rotation *= Quaternion.AngleAxis(rotateY * rotateSpeed, Vector3.right);
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
        transform.rotation = Quaternion.Euler(0, followTarget.rotation.eulerAngles.y, 0);
        followTarget.localEulerAngles = new Vector3(angles.x, 0, 0);
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

    void ChangeFingers()
    {
        if (isRunning)
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

    public void ChangeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    public void GameOver()
    {
        gameOver = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameManager.GameOver(gameOver);
    }

    private void check()
    {
        Collider[] colliders01 = Physics.OverlapSphere(hip.transform.position, 1, LayerMask.GetMask("EASY"));
        if (colliders01.Length > 0)
        {
            Debug.Log("easy");
            gameManager.ChoseEasy();
        }

        Collider[] colliders02 = Physics.OverlapSphere(hip.transform.position, 1, LayerMask.GetMask("NORMAL"));
        if (colliders02.Length > 0)
        {
            Debug.Log("normal");
            gameManager.ChoseNormal();
        }

        Collider[] colliders03 = Physics.OverlapSphere(hip.transform.position, 1, LayerMask.GetMask("HARD"));
        if (colliders03.Length > 0)
        {
            Debug.Log("hard");
            gameManager.ChoseHard();
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        // Debug.Log("Jump! " + hip.transform.position.y.ToString());
        if (hip.transform.position.y < 1.0f)
        {
            Rigidbody rbhip = hip.GetComponent<Rigidbody>();
            // rbhip.velocity += new Vector3(Random.Range(-1f, 1f), 30f, Random.Range(-1f, 1f));
            rbhip.velocity += new Vector3(0f, 50f, 0f);
            // rbhip.angularVelocity += new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
            rbhip.angularVelocity += new Vector3(5f, 5f, 5f);
        }
    }
}
