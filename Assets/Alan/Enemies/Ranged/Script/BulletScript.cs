using System;
using System.Collections;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        StartCoroutine(BulletDespawn());
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerStats.Instance.TakeDamage(EnemyRangedShooting.BulletDamageReference);
            Destroy(gameObject);
        }
    }

    private IEnumerator BulletDespawn()
    {
        yield return new WaitForSeconds(EnemyRangedShooting.BulletDespawnTimeReference);
        Destroy(gameObject);
    }
}
