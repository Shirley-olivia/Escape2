//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TargetCollision : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        var foot = transform;
//        var ray = new Ray(foot.transform.position + Vector3.up * 0.5f, Vector3.down);
//        var hitInfo = new RaycastHit();
//        if (Physics.SphereCast(ray, 0.05f, out hitInfo, 0.50f))
//            foot.position = hitInfo.point + Vector3.up * 0.05f;
//    }
//}
