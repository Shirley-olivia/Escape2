using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionController : MonoBehaviour
{
    Animator animator;
    Renderer[] characterMaterials;

    public Texture2D[] albedoList;
    [ColorUsage(true,true)]
    public Color[] eyeColors;
    public enum EyePosition { normal, happy, angry, dead}
    public EyePosition eyeState;
    private Color originEyeColor;
    public int materialIndex = 1;

    public FingerController fingerController;
    bool LHand = true;
    bool RHand = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        characterMaterials = GetComponentsInChildren<Renderer>();
        ChangeMaterialSettings(materialIndex);
    }

    public void setEmotion(string emotion)
    {
        switch (emotion)
        {
            case "normal":
                ChangeEyeOffset(EyePosition.normal);
                ChangeAnimatorIdle("normal");
                if(!LHand)
                {
                    fingerController.Expand(FingerController.Hand.Left);
                    LHand = true;
                }
                if (!RHand)
                {
                    fingerController.Expand(FingerController.Hand.Right);
                    RHand = true;
                }
                break;
            case "angry":
                ChangeEyeOffset(EyePosition.angry);
                ChangeAnimatorIdle("angry");
                if (LHand)
                {
                    //Debug.Log("LShrink");
                    fingerController.Shrink(FingerController.Hand.Left);
                    LHand = false;
                }
                if (RHand)
                {
                    //Debug.Log("RShrink");
                    fingerController.Shrink(FingerController.Hand.Right);
                    RHand = false;
                }
                break;
            case "dead":
                ChangeEyeOffset(EyePosition.dead);
                ChangeAnimatorIdle("dead");
                StartCoroutine(DelayedAction());
                break;
        }
    }

    void ChangeAnimatorIdle(string trigger)
    {
        animator.SetTrigger(trigger);
    }

    void ChangeMaterialSettings(int index)
    {
        for (int i = 0; i < characterMaterials.Length; i++)
        {
            if (characterMaterials[i].transform.CompareTag("PlayerEyes"))
            {
                characterMaterials[i].material.SetColor("_EmissionColor", eyeColors[index]);
                originEyeColor = eyeColors[index];
            }   
            else if (!characterMaterials[i].transform.CompareTag("Pole"))
                characterMaterials[i].material.SetTexture("_MainTex", albedoList[index]);
        }
    }

    void ChangeEyeOffset(EyePosition pos)
    {
        Vector2 offset = Vector2.zero;

        switch (pos)
        {
            case EyePosition.normal:
                offset = new Vector2(0, 0);
                break;
            case EyePosition.happy:
                offset = new Vector2(.33f, 0);
                break;
            case EyePosition.angry:
                offset = new Vector2(.66f, 0);
                break;
            case EyePosition.dead:
                offset = new Vector2(.33f, .66f);
                break;
            default:
                break;
        }

        for (int i = 0; i < characterMaterials.Length; i++)
        {
            if (characterMaterials[i].transform.CompareTag("PlayerEyes"))
                characterMaterials[i].material.SetTextureOffset("_MainTex", offset);
        }
    }

    public void setEyeColor(float radio)
    {
        for (int i = 0; i < characterMaterials.Length; i++)
        {
            if (characterMaterials[i].transform.CompareTag("PlayerEyes"))
            {
                Color color = originEyeColor * Mathf.Pow(2, (-5 + 5 * radio));
                color.a = originEyeColor.a;
                characterMaterials[i].material.SetColor("_EmissionColor", color);
            }
        }
    }

    private IEnumerator DelayedAction()
    {
        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
    }
}
