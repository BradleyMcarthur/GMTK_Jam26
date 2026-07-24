using System;
using System.Collections;
using UnityEngine;

public class EnemyRangedShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform shootingPoint;
    public static int BulletDamageReference;
    public static float BulletDespawnTimeReference;

    [Space(10)]
    public float shootingCooldown;
    public float currentShootingCooldown;
    
    [Space(10)]
    public float bulletSpeed;
    public int bulletDamage;
    public float bulletDespawnTime;
    

    private void Awake()
    {
        currentShootingCooldown = shootingCooldown;
        BulletDamageReference = bulletDamage;
        BulletDespawnTimeReference = bulletDespawnTime;
    }

    void Update()
    {
        if (currentShootingCooldown > 0)
        {
            currentShootingCooldown -= Time.deltaTime;
            return;
        }
        currentShootingCooldown = shootingCooldown;
        EnemyRangedLoopedShooting();
    }

    private void EnemyRangedLoopedShooting()
    {
        GameObject newBullet = Instantiate(bulletPrefab, shootingPoint.transform.position, shootingPoint.transform.rotation);
        newBullet.GetComponent<Rigidbody2D>().linearVelocity = transform.up * bulletSpeed;
    }
}
