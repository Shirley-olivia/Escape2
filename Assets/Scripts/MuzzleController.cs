using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleController : MonoBehaviour
{
    public float shootRate;
    public float shootRange;
    public GameObject bullet;
    public float bulletSpeed;
    private float lastShootTime;
    [HideInInspector] public bool isFiring;
    public Camera mainCamera;
    public float damage = 2f;
    public AudioSource lazerAudio;

    void Update()
    {
        if (isFiring)
        {
            if (Time.time - lastShootTime > shootRate)
            {
                Shoot();
                lastShootTime = Time.time;
            }
        }
    }

    private void Shoot()
    {
        lazerAudio.Play();
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, shootRange, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            Vector3 targetPoint = hit.point;
            Vector3 direction = targetPoint - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            GameObject bulletClone = Instantiate(bullet, transform.position, rotation);
            bulletClone.GetComponent<ShotBehavior>().setTarget(targetPoint);
            GameObject.Destroy(bulletClone, 2f);
            if (hit.transform.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hit.transform.GetComponent<EnemyHealth>();
                if (enemyHealth)
                {
                    enemyHealth.TakeDamage(damage, direction.normalized);
                }
            }
        }
        else
        {
            Vector3 direction = mainCamera.transform.forward;
            Quaternion rotation = Quaternion.LookRotation(direction);
            GameObject bulletClone = Instantiate(bullet, transform.position, rotation);
            GameObject.Destroy(bulletClone, 2f);
        }
    }
}
