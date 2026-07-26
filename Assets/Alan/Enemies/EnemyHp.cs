using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyHp : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject resourcePickupPrefab;
    
    public int enemyHealth;
    public int maxHealth = 100;
    public bool isDead = false;

    void Start()
    {
        enemyHealth = maxHealth;
        EnemyManager.Instance.AddEnemyToManagerList(this);
    }

    public void EnemyTakeDamage(int damage)
    {
        if (isDead) return;
        
        enemyHealth -= damage;
        
        if (enemyHealth <= 0)
        {
            isDead = true;
            EnemyManager.Instance.EnemyDied(this);
            KillCounter.Instance?.RegisterKill();
            Die();
        }
    }
    
    public void TakeDamage(float amount)
    {
        GetComponent<HitFlashController>()?.Flash();
        EnemyTakeDamage(Mathf.RoundToInt(amount));
    }
    
    private void Die()
    {
        Instantiate(resourcePickupPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        EnemyManager.Instance.RemoveDeadEnemyFromList(this);
    }
}