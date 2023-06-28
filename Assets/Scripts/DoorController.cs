using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorController : MonoBehaviour
{
    private DoorInputAction inputAction;
    private InputAction grabAction;
    public bool DoorStatus = false;
    public GameObject RightArmTarget;
    public PlayerController player;
    public GameObject ClosedDoor;
    public float DoorSpeed = 1f;

    private float targetZ;
    private Vector3 targetPosition;

    void Awake()
    {
        inputAction = new DoorInputAction();
    }

    void OnEnable()
    {
        inputAction.Enable();
        grabAction = inputAction.Door.Grab;
        grabAction.performed += DoorKey;
    }

    void Start()
    {
        targetZ = ClosedDoor.transform.position.z + 6f;
        targetPosition = new Vector3(ClosedDoor.transform.position.x, ClosedDoor.transform.position.y, targetZ);
    }

    void Update()
    {
        DoorOpen();
    }

    private void DoorKey(InputAction.CallbackContext context)
    {
        float distance = Vector3.Distance(RightArmTarget.transform.position, gameObject.transform.position);
        if (!DoorStatus && distance < 1f)
        {
            player.Grab(gameObject, true);
            DoorStatus = true;
            ClosedDoor.GetComponent<DoorAudio>().PlayAudio();
        }
        else if (DoorStatus)
        {
            player.Grab(gameObject, false);
            DoorStatus = false;
            ClosedDoor.GetComponent<DoorAudio>().StopAudio();
        }
    }

    private void DoorOpen()
    {
        if (DoorStatus)
        {
            ClosedDoor.transform.position = Vector3.Lerp(ClosedDoor.transform.position, targetPosition, DoorSpeed * Time.deltaTime);
        }
    }
}
