using System;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public static Rigidbody2D BulletRigidBody;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerStatChecks.TakeDamage(EnemyRangedShooting.BulletDamage);
            Destroy(gameObject);
        }
    }
}
