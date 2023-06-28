#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKSolver : MonoBehaviour
{
    public Transform target;
    public List<Transform> bones;
    public List<Transform> poles;
    public int iterations = 1000;
    public float deltaDistance = 0.0001f;
    [Range(0, 1)]
    public float snapBackStrength = 1f;
    private float[] boneLengths;
    private Vector3[] startDirections;
    private Quaternion[] startRotations;
    private Quaternion startRotationTarget;
    private Transform root;
    private float totalLength;
    private Vector3[] positions;

    void Start()
    {
        root = bones[0].parent;
        if (target == null) {
            target = new GameObject(gameObject.name + " Target").transform;
            SetPositionRootSpace(target, GetPositionRootSpace(transform));
        }
        startRotationTarget = GetRotationRootSpace(target);
        boneLengths = new float[bones.Count - 1];
        startDirections = new Vector3[bones.Count];
        startRotations = new Quaternion[bones.Count];
        positions = new Vector3[bones.Count];
        totalLength = 0f;
        for (int i = bones.Count - 1; i >= 0; i--) {
            startRotations[i] = GetRotationRootSpace(bones[i]);
            if (i == bones.Count - 1) {
                startDirections[i] = GetPositionRootSpace(target) - GetPositionRootSpace(bones[i]);
            } else {
                startDirections[i] = GetPositionRootSpace(bones[i + 1]) - GetPositionRootSpace(bones[i]);
                boneLengths[i] = startDirections[i].magnitude;
                totalLength += boneLengths[i];
            }
        }
    }

    void LateUpdate()
    {
        ResolveIK();
    }

    private void ResolveIK()
    {
        if (target == null) {
            return;
        }
        for (int i = 0; i < bones.Count; i++) {
            positions[i] = GetPositionRootSpace(bones[i]);
        }
        Vector3 targetPosition = GetPositionRootSpace(target);
        Quaternion targetRotation = GetRotationRootSpace(target);
        if (Vector3.Distance(positions[0], targetPosition) >= totalLength) {
            Vector3 direction = (targetPosition - positions[0]).normalized;
            for (int i = 1; i < positions.Length; i++) {
                positions[i] = positions[i - 1] + direction * boneLengths[i - 1];
            }
        }
        else {
            for (int i = 0; i < positions.Length - 1; i++)
                positions[i + 1] = Vector3.Lerp(positions[i + 1], positions[i] + startDirections[i], snapBackStrength);
            for (int iteration = 0; iteration < iterations; iteration++) {
                for (int i = positions.Length - 1; i > 0; i--) {
                    if (i == positions.Length - 1)
                        positions[i] = targetPosition;
                    else
                        positions[i] = positions[i + 1] + (positions[i] - positions[i + 1]).normalized * boneLengths[i];
                }
                for (int i = 1; i < positions.Length; i++)
                    positions[i] = positions[i - 1] + (positions[i] - positions[i - 1]).normalized * boneLengths[i - 1];
                if (Vector3.Distance(positions[positions.Length - 1], targetPosition) < deltaDistance)
                    break;
            }
        }
        for (int i = 1; i < positions.Length - 1; i++) {
            if (poles[i] != null) {
                Vector3 polePosition = GetPositionRootSpace(poles[i]);
                Plane plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
                Vector3 projectedPole = plane.ClosestPointOnPlane(polePosition);
                Vector3 projectedBone = plane.ClosestPointOnPlane(positions[i]);
                float angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1], plane.normal);
                positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) + positions[i - 1];
            }
        }
        for (int i = 0; i < positions.Length; i++)
        {
            if (i == positions.Length - 1)
                SetRotationRootSpace(bones[i], Quaternion.Inverse(targetRotation) * startRotationTarget * Quaternion.Inverse(startRotations[i]));
            else
                SetRotationRootSpace(bones[i], Quaternion.FromToRotation(startDirections[i], positions[i + 1] - positions[i]) * Quaternion.Inverse(startRotations[i]));
            SetPositionRootSpace(bones[i], positions[i]);
        }
    }

    private Vector3 GetPositionRootSpace(Transform current)
    {
        if (root == null)
            return current.position;
        else
            return Quaternion.Inverse(root.rotation) * (current.position - root.position);
    }

    private void SetPositionRootSpace(Transform current, Vector3 position)
    {
        if (root == null)
            current.position = position;
        else
            current.position = root.rotation * position + root.position;
    }

    private Quaternion GetRotationRootSpace(Transform current)
    {
        if (root == null)
            return current.rotation;
        else
            return Quaternion.Inverse(current.rotation) * root.rotation;
    }

    private void SetRotationRootSpace(Transform current, Quaternion rotation)
    {
        if (root == null)
            current.rotation = rotation;
        else
            current.rotation = root.rotation * rotation;
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        for (int i = 0; i < bones.Count - 1; i++) {
            float scale = Vector3.Distance(bones[i].position, bones[i + 1].parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(bones[i].position, Quaternion.FromToRotation(Vector3.up, bones[i + 1].position - bones[i].position), new Vector3(scale, Vector3.Distance(bones[i + 1].position, bones[i].position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
        }
#endif
    }
}
