using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActiveTrigger : MonoBehaviour
{
    private GameObject enemy;
    void Update()
    {
        if (enemy != null)
        {
            transform.position = enemy.transform.position;
        }
    }
    public void SetEnemy(GameObject enemy)
    {
        this.enemy = enemy;
    }
    public void SetEnemyActive(bool active)
    {
        enemy.SetActive(active);
    }
}
