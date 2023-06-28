using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class MinimapMove : MonoBehaviour
{
    private MinimapInputAction inputAction;
    private InputAction maxAction;
    private InputAction zoomAction;
    public Transform player;
    [Header("PlayerArrow")]
    public Transform miniPlayerIcon;
    public Transform maxPlayerIcon;
    [Header("MapSize")]
    public float minSize;
    public float maxSize;
    [Header("Map")]
    public GameObject miniMap;
    public GameObject maxMap;


    private float mapSize;
    private Camera minimapCamera;

    void Awake()
    {
        inputAction = new MinimapInputAction();
    }

    void OnEnable()
    {
        inputAction.Enable();
        maxAction = inputAction.MiniMap.Max;
        maxAction.performed += ChangeMapView;
        zoomAction = inputAction.MiniMap.Zoom;
    }

    void Start()
    {
        minimapCamera = GetComponent<Camera>();
        mapSize = minimapCamera.orthographicSize;
        miniMap.SetActive(true);
        maxMap.SetActive(false);
    }

    void Update()
    {
        transform.position = player.position + new Vector3(0, 20, 0);
        if (miniMap.activeSelf)
        {
            miniPlayerIcon.eulerAngles = new Vector3(0, 0, -player.eulerAngles.y);
            changeMapSize(-zoomAction.ReadValue<Vector2>().y / 120);
        }
        else
        {
            maxPlayerIcon.eulerAngles = new Vector3(0, 0, -player.eulerAngles.y);
        }
        
    }

    private void ChangeMapView(InputAction.CallbackContext context)
    {
        if (miniMap.activeSelf)
        {
            minimapCamera.cullingMask = 1 << 8;
            miniMap.SetActive(false);
            maxMap.SetActive(true);
            minimapCamera.orthographicSize = 50;
        }
        else
        {
            minimapCamera.cullingMask = 1 << 8 | 1 << 9 | 1 << 17;
            miniMap.SetActive(true);
            maxMap.SetActive(false);
            minimapCamera.orthographicSize = mapSize;
        }
    }

    private void changeMapSize(float value)
    {
        mapSize += value;
        mapSize = Mathf.Clamp(mapSize, minSize, maxSize);
        minimapCamera.orthographicSize = mapSize;
    }
}
