using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMove : MonoBehaviour
{
    public Vector3 Dir;
    public float speed = 1.0f;
    private float time;
    public float bias;
    private Vector3 pre_positon;
    private Vector3 new_position;

    private void Awake()
    {
        pre_positon = transform.position;
        time = bias * Mathf.PI;
    }



    void Update()
    {
        time += Time.deltaTime * speed;

        if (time < Mathf.PI)
        {
            new_position += Time.deltaTime * Mathf.Cos(time) * Dir;
            transform.position = new_position + transform.parent.position;
            pre_positon = transform.position;
        }
        else if (time > 2 * Mathf.PI)
        {
            time = 0;
        }
        else
        {
            transform.position = pre_positon;
        }



        //Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        //RaycastHit hitInfo = new RaycastHit();
        //if (Physics.SphereCast(ray, 0.05f, out hitInfo, 0.50f) && new_position.y < hitInfo.point.y)
        //{
        //    new_position.y = transform.position.y;
        //}
    }
}
