using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHP = 20f;

    private bool damaged = false;
    public float curHP;
    private Vector3 damagedDirection;
    private EmotionController emotionController;


    void Start()
    {
        damaged = false;
        curHP = maxHP;
        emotionController = GetComponent<EmotionController>();
    }

    public bool getDamaged()
    {
        return damaged;
    }

    public void setDamaged(bool state)
    {
        damaged = state;
    }

    public Vector3 getDamagedDirection()
    {
        return damagedDirection;
    }

    public void TakeDamage(float damage, Vector3 rayDirection)
    {
        damaged = true;
        curHP -= damage;
        //改变眼睛颜色
        emotionController.setEyeColor(curHP / maxHP);
        //返回从敌人出发的方向
        damagedDirection = -rayDirection;
    }

    public bool isAlive()
    {
        return curHP > 0;
    }
}
