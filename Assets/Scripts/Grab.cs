using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    public Transform[] fingerTips; // 存储每个手指末端位置的Transform
    public LayerMask objectLayer; // 物体的图层
    public GameObject targetObject; // 目标物体
    public float moveSpeed = 0.1f; // 移动速度
    public IKSolver hand;

    private IKSolver[] fingerIKFabrics; // 存储手指的FastIKFabric组件
    private bool handMove = true;

    void Start()
    {
        // 获取所有手指上的FastIKFabric组件
        fingerIKFabrics = GetComponentsInChildren<IKSolver>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, targetObject.transform.position);
        //print("diatance: " + distance);
        if (distance > 0.3f && handMove == true)
        {
            // 获取物体与 this.transform 连线的方向
            Vector3 direction = (targetObject.transform.position - hand.target.position).normalized;
            print("direction: " + direction);
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.left).normalized;
            print("perpendicular: " + perpendicular);
            Quaternion targetRotation = Quaternion.LookRotation(direction, -perpendicular);
            print("targetRotation: " + targetRotation);
            // 移动当前物体，并旋转使其与目标物体相切
            hand.target.position += direction * moveSpeed * Time.deltaTime;
            Quaternion adjustedRotation = Quaternion.Euler(-120f, 0f, 0f);
            hand.target.rotation = targetRotation * adjustedRotation;
        }
        //else
        //{
        //    handMove = false;
        //    for (int i = 0; i < fingerTips.Length; i++)
        //    {
        //        Vector3 fingerPosition = fingerTips[i].position;
        //        Vector3 fingerForward = fingerTips[i].forward;
        //        print("a: " + fingerForward);

        //        // 射线检测物体表面碰撞点
        //        RaycastHit hit;
        //        bool intersected = false;

        //        if(!intersected)
        //        {
        //            // 尝试直接与物体相交
        //            if (Physics.Raycast(fingerPosition, fingerForward, out hit, Mathf.Infinity, objectLayer))
        //            {
        //                fingerIKFabrics[i].target.position = hit.point;
        //                intersected = true;
        //            }
        //            //else
        //            //{
        //            //    // 如果没有相交，则微微蜷缩手指再尝试相交
        //            //    for (float s = 0.1f; s <= 1.0f; s += 0.1f)
        //            //    {
        //            //        Vector3 curledFingerPosition = fingerPosition + fingerForward * s * 0.1f;

        //            //        if (Physics.Raycast(curledFingerPosition, fingerForward, out hit, Mathf.Infinity, objectLayer))
        //            //        {
        //            //            fingerIKFabrics[i].target.position = hit.point;
        //            //            intersected = true;
        //            //            break;
        //            //        }
        //            //    }
        //            //}

        //            //if (!intersected)
        //            else
        //            {
        //                // 如果还是没有相交，则将手指的目标位置设置为射线的末端位置
        //                fingerIKFabrics[i].target.position = fingerPosition + fingerForward * 0.005f;
        //            }

        //            print(i + ": " + fingerIKFabrics[i].target.position);
        //        }
                
        //    }
        //}
    }
}
