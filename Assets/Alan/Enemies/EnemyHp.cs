using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyHp : MonoBehaviour, IDamageable
{
    [SerializeField] GameObject resourcePickupPrefab;
    
    public float enemyHealth;
    public float maxHealth = 100;
    public bool isDead = false;

    void Start()
    {
        enemyHealth = maxHealth;
        EnemyManager.Instance.AddEnemyToManagerList(this);
    }

    // private void Update() for testing purposes
    // {
    //     if (Keyboard.current.spaceKey.wasPressedThisFrame)
    //     {
    //         EnemyTakeDamage(10);
    //     }
    // }

    public void EnemyTakeDamage(float damage)
    {
        if (isDead) return;
        
        enemyHealth -= damage;
        
        if (enemyHealth <= 0)
        {
            isDead = true;
            EnemyManager.Instance.EnemyDied(this);
            Die();
        }
    }
    
    public void TakeDamage(float amount)
    {
        EnemyTakeDamage(amount);
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
