using System;
using System.Collections;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private bool hitPlayer = false;
    [SerializeField] private float despawnTimerCheck; 

    private void Awake()
    {
        hitPlayer = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        StartCoroutine(DespawnMeleeAttack());

        if (other.gameObject.CompareTag("Player") && !hitPlayer)
        {
            hitPlayer = true;
            PlayerStatChecks.TakeDamage(EnemyMeleeAttack.meleeDamageReference);
        }
    }

    private IEnumerator DespawnMeleeAttack()
    {
        despawnTimerCheck = EnemyMeleeAttack.meleeDespawnTimeReference;
        yield return new WaitForSeconds(EnemyMeleeAttack.meleeDespawnTimeReference);
        Destroy(gameObject);
    }
}
