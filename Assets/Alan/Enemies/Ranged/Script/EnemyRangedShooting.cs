using System.Collections;
using UnityEngine;

public class EnemyRangedShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject shootingPoint;
    
    public float bulletSpeed;
    public static int BulletDamage;
    public float shootingCooldown;
    public bool isInCooldown = false;
    
    void FixedUpdate()
    {
        if (!gameObject) return;
        StartCoroutine(EnemyRangedLoopedShooting());
    }

    private IEnumerator EnemyRangedLoopedShooting() // fix the weird shooting timing
    {
        if (isInCooldown) yield break;
        yield return new WaitForSeconds(shootingCooldown);
        
        GameObject newBullet = Instantiate(bulletPrefab, shootingPoint.transform.position, shootingPoint.transform.rotation);
        Rigidbody2D newBulletRb = newBullet.GetComponent<Rigidbody2D>();

        if (newBulletRb)
        {
            newBulletRb.linearVelocity = transform.up * bulletSpeed;
        }
        isInCooldown = true;
    }
}
