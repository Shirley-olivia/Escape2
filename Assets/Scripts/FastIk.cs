using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastIk : MonoBehaviour
{
    public Transform target;
    public Transform pole;
    public Transform[] bones;
    public float chainLength = 3f;
    public int iterations = 10;
    public float deltaThreshold = 0.001f;

    private float[] bonesLength;
    private float completeLength;
    private Vector3[] bonesPositions;
    private Vector3[] startDirectionSucc;
    private Quaternion[] startRotationBone;
    private Quaternion startRotationTarget;
    private Transform root;

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        bonesLength = new float[bones.Length];
        bonesPositions = new Vector3[bones.Length + 1];
        startDirectionSucc = new Vector3[bones.Length + 1];
        startRotationBone = new Quaternion[bones.Length];
        root = transform;

        completeLength = 0;

        for (int i = 0; i < bones.Length; i++)
        {
            bonesPositions[i] = bones[i].position;
            startRotationBone[i] = bones[i].rotation;

            if (i == 0)
            {
                startDirectionSucc[i] = target.position - bonesPositions[i];
            }
            else
            {
                startDirectionSucc[i] = bonesPositions[i - 1] - bonesPositions[i];
                bonesLength[i - 1] = startDirectionSucc[i].magnitude;
                completeLength += bonesLength[i - 1];
            }
        }

        bonesPositions[bones.Length] = bones[bones.Length - 1].position;
        startRotationTarget = target.rotation;
    }

    void LateUpdate()
    {
        ResolveIK();
    }

    void ResolveIK()
    {
        if (target == null)
            return;

        if (bonesLength.Length != bones.Length)
            Initialize();

        for (int i = 0; i < bones.Length; i++)
        {
            bonesPositions[i] = bones[i].position;
        }

        if ((target.position - bonesPositions[0]).sqrMagnitude >= completeLength * completeLength)
        {
            Vector3 direction = (target.position - bonesPositions[0]).normalized;
            for (int i = 1; i < bonesPositions.Length; i++)
            {
                bonesPositions[i] = bonesPositions[i - 1] + direction * bonesLength[i - 1];
            }
        }
        else
        {
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int i = bonesPositions.Length - 1; i > 0; i--)
                {
                    if (i == bonesPositions.Length - 1)
                    {
                        bonesPositions[i] = target.position;
                    }
                    else
                    {
                        bonesPositions[i] = bonesPositions[i + 1] + (bonesPositions[i] - bonesPositions[i + 1]).normalized * bonesLength[i];
                    }
                }

                for (int i = 1; i < bonesPositions.Length; i++)
                {
                    bonesPositions[i] = bonesPositions[i - 1] + (bonesPositions[i] - bonesPositions[i - 1]).normalized * bonesLength[i - 1];
                }

                if ((bonesPositions[bonesPositions.Length - 1] - target.position).sqrMagnitude < deltaThreshold * deltaThreshold)
                    break;
            }
        }

        if (pole != null)
        {
            for (int i = 1; i < bonesPositions.Length - 1; i++)
            {
                Plane plane = new Plane(bonesPositions[i + 1] - bonesPositions[i - 1], bonesPositions[i - 1]);
                Vector3 projectedPole = plane.ClosestPointOnPlane(pole.position);
                Vector3 projectedBone = plane.ClosestPointOnPlane(bonesPositions[i]);
                float angle = Vector3.SignedAngle(projectedBone - bonesPositions[i - 1], projectedPole - bonesPositions[i - 1], plane.normal);
                bones[i].rotation = Quaternion.AngleAxis(angle, plane.normal) * bones[i].rotation;
            }
        }
        else
        {
            for (int i = 0; i < bones.Length - 1; i++)
            {
                bones[i].rotation = Quaternion.FromToRotation(startDirectionSucc[i], bonesPositions[i + 1] - bonesPositions[i]) * startRotationBone[i];
            }
        }

        target.rotation = startRotationTarget;
    }
}
