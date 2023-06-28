using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerController : MonoBehaviour
{
    public enum Hand
    {
        Left,
        Right,
        Both
    }

    public GameObject leftFingerIndexTarget;
    public GameObject leftFingerMiddleTarget;
    public GameObject leftFingerPinkyTarget;
    public GameObject leftFingerThumbTarget;
    public GameObject leftHand;
    public GameObject leftThumb;

    public GameObject rightFingerIndexTarget;
    public GameObject rightFingerMiddleTarget;
    public GameObject rightFingerPinkyTarget;
    public GameObject rightFingerThumbTarget;
    public GameObject rightHand;
    public GameObject rightThumb;

    public bool isDebug = true;

    private bool isQKeyPressed = true;
    public float rotationSpeed = 30f;
    public float rotationSpeedT = 40f;

    public void Shrink(Hand hand)
    {
        StartCoroutine(CoroutineShrink(hand));
    }

    public void Expand(Hand hand)
    {
        StartCoroutine(CoroutineExpand(hand));
    }

    IEnumerator CoroutineShrink(Hand hand)
    {
        float endTime = Time.time + 0.05f;
        while (Time.time < endTime)
        {
            float rotationAngle = rotationSpeed * Time.deltaTime;
            float rotationAngleT = rotationSpeedT * Time.deltaTime;

            if (hand == Hand.Left || hand == Hand.Both)
            {
                Vector3 axisPositionL = leftHand.transform.position;
                Vector3 axisDirectionL = -leftHand.transform.right;
                leftFingerIndexTarget.transform.RotateAround(axisPositionL, axisDirectionL, rotationAngle);
                leftFingerMiddleTarget.transform.RotateAround(axisPositionL, axisDirectionL, rotationAngle);
                leftFingerPinkyTarget.transform.RotateAround(axisPositionL, axisDirectionL, rotationAngle);

                Vector3 axisPositionLT = leftThumb.transform.position;
                Vector3 axisDirectionLT = leftThumb.transform.up;
                leftFingerThumbTarget.transform.RotateAround(axisPositionLT, axisDirectionLT, rotationAngleT);
            }

            if (hand == Hand.Right || hand == Hand.Both)
            {
                Vector3 axisPositionR = rightHand.transform.position;
                Vector3 axisDirectionR = rightHand.transform.right;
                rightFingerIndexTarget.transform.RotateAround(axisPositionR, axisDirectionR, rotationAngle);
                rightFingerMiddleTarget.transform.RotateAround(axisPositionR, axisDirectionR, rotationAngle);
                rightFingerPinkyTarget.transform.RotateAround(axisPositionR, axisDirectionR, rotationAngle);

                Vector3 axisPositionRT = rightThumb.transform.position;
                Vector3 axisDirectionRT = rightThumb.transform.up;
                rightFingerThumbTarget.transform.RotateAround(axisPositionRT, axisDirectionRT, rotationAngleT);
            }
            yield return null;
        }
    }

    IEnumerator CoroutineExpand(Hand hand)
    {
        float endTime = Time.time + 0.05f;
        while (Time.time < endTime)
        {
            float rotationAngle = rotationSpeed * Time.deltaTime;
            float rotationAngleT = rotationSpeedT * Time.deltaTime;

            if (hand == Hand.Left || hand == Hand.Both)
            {
                Vector3 axisPositionL = leftHand.transform.position;
                Vector3 axisDirectionL = -leftHand.transform.right;
                leftFingerIndexTarget.transform.RotateAround(axisPositionL, axisDirectionL, -rotationAngle);
                leftFingerMiddleTarget.transform.RotateAround(axisPositionL, axisDirectionL, -rotationAngle);
                leftFingerPinkyTarget.transform.RotateAround(axisPositionL, axisDirectionL, -rotationAngle);

                Vector3 axisPositionLT = leftThumb.transform.position;
                Vector3 axisDirectionLT = leftThumb.transform.up;
                leftFingerThumbTarget.transform.RotateAround(axisPositionLT, axisDirectionLT, -rotationAngleT);
            }

            if (hand == Hand.Right || hand == Hand.Both)
            {
                Vector3 axisPositionR = rightHand.transform.position;
                Vector3 axisDirectionR = rightHand.transform.right;
                rightFingerIndexTarget.transform.RotateAround(axisPositionR, axisDirectionR, -rotationAngle);
                rightFingerMiddleTarget.transform.RotateAround(axisPositionR, axisDirectionR, -rotationAngle);
                rightFingerPinkyTarget.transform.RotateAround(axisPositionR, axisDirectionR, -rotationAngle);

                Vector3 axisPositionRT = rightThumb.transform.position;
                Vector3 axisDirectionRT = rightThumb.transform.up;
                rightFingerThumbTarget.transform.RotateAround(axisPositionRT, axisDirectionRT, -rotationAngleT);
            }
            yield return null;
        }
    }
}
