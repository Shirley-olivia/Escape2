using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    public float sensorInterval = 0.2f;
    [Space]
    public float hearingRange = 20f;
    public float visionRange = 40f;
    public float visionAngle = 120f;

    private GameObject player;
    private GameObject playerLocked;
    private GameObject bulletLocked;
    private float sensorTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        sensorTimer = sensorInterval;
    }

    private void FixedUpdate()
    {
        if (sensorTimer >= sensorInterval)
        {
            sensorUpdate();
            sensorTimer = 0f;
        }
        sensorTimer += Time.deltaTime;
    }

    private void sensorUpdate()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= hearingRange)
        {
            playerLocked = player;
        }
        else if (distance <= visionRange)
        {
            Vector3 direction = player.transform.position - transform.position;
            float angle = Mathf.Abs(Vector3.Angle(transform.forward, direction));

            if (angle < visionAngle / 2)
            {
                Ray ray = new Ray(transform.position, direction);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, visionRange))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        playerLocked = player;
                    }
                }
            }
        }
        else
        {
            playerLocked = null;

            Collider[] colliders = Physics.OverlapSphere(transform.position, hearingRange * 3, LayerMask.GetMask("Bullet"));
            if (colliders.Length > 0)
            {
                bulletLocked = colliders[0].gameObject;
            }
            else
            {
                bulletLocked = null;
            }
        }
    }

    public GameObject getLockedPlayer()
    {
        return playerLocked;
    }

    public GameObject getLockedBullet()
    {
        return bulletLocked;
    }
}
