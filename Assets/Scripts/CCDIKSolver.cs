using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CCDIKSolver : MonoBehaviour
{
    public Transform target;
    public Transform[] bones;
    private float[] boneLengths;
    private Vector3[] boneAxis;
    public float IKPositionWeight = 1f;
    public int maxIterations = 4;
    public bool useRotationLimits = true;
    public float tolerance = 0f;
    protected Vector3 lastLocalDirection;
    private float[] boneWeights;

    private Vector3 IKPosition;
    private float chainLength;

    void Start()
    {
        IKPosition = bones[bones.Length - 1].transform.position;
        boneLengths = new float[bones.Length - 1];
        boneAxis = new Vector3[bones.Length];
        boneWeights = new float[bones.Length];
        InitiateBones();
        FadeOutBoneWeights();
    }

    void LateUpdate()
    {
        if (IKPositionWeight <= 0) return;
        IKPositionWeight = Mathf.Clamp(IKPositionWeight, 0f, 1f);

        if (target != null) IKPosition = target.position;

        Vector3 singularityOffset = maxIterations > 1 ? GetSingularityOffset() : Vector3.zero;

        for (int i = 0; i < maxIterations; i++)
        {
            Solve(IKPosition + (i == 0 ? singularityOffset : Vector3.zero));
        }
    }

    void InitiateBones()
    {
        chainLength = 0;

        for (int i = 0; i < bones.Length; i++)
        {
            if (i < bones.Length - 1)
            {
                boneLengths[i] = (bones[i].transform.position - bones[i + 1].transform.position).magnitude;
                chainLength += boneLengths[i];

                Vector3 nextPosition = bones[i + 1].transform.position;
                boneAxis[i] = Quaternion.Inverse(bones[i].transform.rotation) * (nextPosition - bones[i].transform.position);
            }
            else
            {
                boneAxis[i] = Quaternion.Inverse(bones[i].transform.rotation) * (bones[bones.Length - 1].transform.position - bones[0].transform.position);
            }
        }
    }

    Vector3 GetSingularityOffset()
    {
        if (!SingularityDetected()) return Vector3.zero;

        Vector3 IKDirection = (IKPosition - bones[0].transform.position).normalized;

        Vector3 secondaryDirection = new Vector3(IKDirection.y, IKDirection.z, IKDirection.x);

        return Vector3.Cross(IKDirection, secondaryDirection) * boneLengths[bones.Length - 2] * 0.5f;
    }

    private bool SingularityDetected()
    {
        Vector3 toLastBone = bones[bones.Length - 1].transform.position - bones[0].transform.position;
        Vector3 toIKPosition = IKPosition - bones[0].transform.position;

        float toLastBoneDistance = toLastBone.magnitude;
        float toIKPositionDistance = toIKPosition.magnitude;

        if (toLastBoneDistance < toIKPositionDistance) return false;
        if (toLastBoneDistance < chainLength - (boneLengths[bones.Length - 2] * 0.1f)) return false;
        if (toLastBoneDistance == 0) return false;
        if (toIKPositionDistance == 0) return false;
        if (toIKPositionDistance > toLastBoneDistance) return false;

        float dot = Vector3.Dot(toLastBone / toLastBoneDistance, toIKPosition / toIKPositionDistance);
        if (dot < 0.999f) return false;

        return true;
    }

    void Solve(Vector3 targetPosition)
    {
        for (int i = bones.Length - 2; i > -1; i--)
        {
            float w = boneWeights[i] * IKPositionWeight;

            if (w > 0f)
            {
                Vector3 toLastBone = bones[bones.Length - 1].transform.position - bones[i].transform.position;
                Vector3 toTarget = targetPosition - bones[i].transform.position;

                Quaternion targetRotation = Quaternion.FromToRotation(toLastBone, toTarget) * bones[i].transform.rotation;

                if (w >= 1) bones[i].transform.rotation = targetRotation;
                else bones[i].transform.rotation = Quaternion.Lerp(bones[i].transform.rotation, targetRotation, w);
            }
        }
    }

    public void FadeOutBoneWeights()
    {
        if (bones.Length < 2) return;

        boneWeights[0] = 1f;
        float step = 1f / (bones.Length - 1);

        for (int i = 1; i < bones.Length; i++)
        {
            boneWeights[i] = step * (bones.Length - 1 - i);
        }
    }
}
