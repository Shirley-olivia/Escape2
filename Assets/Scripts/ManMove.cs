using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManMove : MonoBehaviour
{
    private Vector3 Start;
    public float speed = 3.0f;
    public float height = 0.05f;

    void Awake()
    {
        Start = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Time.timeSinceLevelLoad * speed;
        transform.position = Start + height * Vector3.up * Mathf.Sin(angle);
        transform.position += transform.forward * angle / 10;
    }
}
