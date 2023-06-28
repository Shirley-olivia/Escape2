using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootGroundingIK : MonoBehaviour
{
    [Header("Body Transforms")]
    public Transform leftFoot;
    public Transform leftLeg;
    public Transform leftThigh;
    public Transform rightFoot;
    public Transform rightLeg;
    public Transform rightThigh;
    public Transform hips;
    [Header("Position Constraints")]
    public Transform leftTarget;
    public Transform leftConstraint;
    public Transform rightTarget;
    public Transform rightConstraint;
    [Header("Lift Up Params")]
    public float leastAngle = 5f;
    public float leastDistance = 0.1f;
    public float liftUpSpeed = 0.1f;
    private int iters = 10;

    void Start()
    {
    }

    void Update()
    {
        LiftUpBody();
        for (int i = 0; i < iters; i++)
            TwoBoneIK();
    }

    private void TwoBoneIK()
    {
        TwoBoneIKOneLeg(leftThigh, leftLeg, leftFoot, leftTarget, leftConstraint);
        TwoBoneIKOneLeg(rightThigh, rightLeg, rightFoot, rightTarget, rightConstraint);
    }

    private void TwoBoneIKOneLeg(Transform thigh, Transform leg, Transform foot, Transform target, Transform constraint)
    {
        float upLength = Vector3.Distance(thigh.position, leg.position);
        float lowLength = Vector3.Distance(leg.position, foot.position);
        float targetLength = Vector3.Distance(thigh.position, target.position);
        Quaternion footRotation = foot.rotation;
        if (targetLength >= upLength + lowLength) {
            Vector3 direction = (target.position - thigh.position).normalized;
            leg.position = thigh.position + direction * upLength;
            foot.position = leg.position + direction * lowLength;
        } else {
            float upAngle = Mathf.Acos((upLength * upLength + targetLength * targetLength - lowLength * lowLength) / (2 * upLength * targetLength)) * Mathf.Rad2Deg;
            Vector3 axis = Vector3.Cross(thigh.position - target.position, thigh.position - constraint.position).normalized;
            Debug.DrawRay(thigh.position, axis, Color.red);
            Vector3 direction = (target.position - thigh.position).normalized;
            direction = (Quaternion.AngleAxis(upAngle, axis) * direction).normalized;
            if (float.IsNaN(direction.x) || float.IsNaN(direction.y) || float.IsNaN(direction.z))
                direction = (target.position - thigh.position).normalized;
            leg.position = thigh.position + direction * upLength;
            foot.position = target.position;
        }
        thigh.up = leg.position - thigh.position;
        leg.up = foot.position - leg.position;
        foot.rotation = footRotation;
    }

    private void LiftUpBody()
    {
        if (!LiftUpFoot(leftTarget) && !LiftUpFoot(rightTarget))
        {
            leftTarget.position += Vector3.down * liftUpSpeed * Time.deltaTime;
            rightTarget.position += Vector3.down * liftUpSpeed * Time.deltaTime;
        }
        if (!IsLegALine(leftThigh, leftLeg, leftFoot) && !IsLegALine(rightThigh, rightLeg, rightFoot))
        {
            hips.position += Vector3.up * liftUpSpeed * Time.deltaTime;
        }
        if (AwayFromTarget(leftFoot, leftTarget) && AwayFromTarget(rightFoot, rightTarget))
        {
            hips.position += Vector3.down * liftUpSpeed * Time.deltaTime;
        }
    }

    private bool LiftUpFoot(Transform target)
    {
        Ray ray = new Ray(target.position + Vector3.up * 0.5f, Vector3.down);
        RaycastHit hit = new RaycastHit();
        if (Physics.SphereCast(ray, 0.05f, out hit, 0.50f))
        {
            target.position = hit.point + Vector3.up * 0.05f;
            return true;
        }
        return false;
    }

    private bool IsLegALine(Transform thigh, Transform leg, Transform foot)
    {
        return Mathf.Abs(Vector3.Angle(leg.position - thigh.position, foot.position - thigh.position)) < leastAngle;
    }

    private bool AwayFromTarget(Transform end, Transform target)
    {
        return Vector3.Distance(end.position, target.position) > leastDistance;
    }
}
