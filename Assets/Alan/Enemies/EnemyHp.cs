using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyHp : MonoBehaviour
{
    public int enemyHealth;
    public int maxHealth = 100;
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

    public void EnemyTakeDamage(int damage)
    {
        if (isDead) return;
        
        enemyHealth -= damage;
        
        if (enemyHealth <= 0)
        {
            isDead = true;
            EnemyManager.Instance.EnemyDied(this);
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        EnemyManager.Instance.RemoveDeadEnemyFromList(this);
    }
}
