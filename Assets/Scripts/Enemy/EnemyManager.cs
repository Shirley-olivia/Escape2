using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public List<Vector3> positions;
    public List<GameObject> enemies;
    public EnemyActiveTrigger trigger;

    private Vector3 getRandonPos(Vector3 centerPos)
    {
        float x = centerPos.x + Random.Range(-5, 5);
        float z = centerPos.z + Random.Range(-5, 5);
        Vector3 tempPos = new Vector3(x, centerPos.y, z);
        tempPos = NavMesh.SamplePosition(tempPos, out NavMeshHit hit, 4, 1) ? hit.position : transform.position;
        return tempPos;
    } 


    public void generate(int intensity)
    {
        for (int i = 0; i < positions.Count; i++)
        {
            int index = Random.Range(0, enemies.Count);
            GameObject instance = Instantiate(enemies[index], positions[i], Quaternion.identity) as GameObject;
            instance.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
            EnemyActiveTrigger triggerInstance = Instantiate<EnemyActiveTrigger>(trigger, positions[i], Quaternion.identity);
            triggerInstance.SetEnemy(instance);
            instance.SetActive(false);
            for (int j = 0; j < intensity; j++)
            {
                index = Random.Range(0, enemies.Count);
                Vector3 pos = getRandonPos(positions[i]);
                GameObject enemy = Instantiate(enemies[index], pos, Quaternion.identity) as GameObject;
                enemy.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
                EnemyActiveTrigger enemyTrigger = Instantiate<EnemyActiveTrigger>(trigger, pos, Quaternion.identity);
                enemyTrigger.SetEnemy(enemy);
                enemy.SetActive(false);
            }
        }
    }
}
