using System;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    public GameObject meleePrefab;
    public Transform meleePoint;
    public static int meleeDamageReference;
    public static float meleeDespawnTimeReference;
    
    public int meleeDamage;
    public float meleeAtkCooldown;
    public float currentMeleeCooldown;
    public float meleeDespawnTime;
    
    public bool isAbleToAttack = false;

    private void Awake()
    {
        currentMeleeCooldown = meleeAtkCooldown;
        meleeDamageReference = meleeDamage; 
        meleeDespawnTimeReference = meleeDespawnTime;
    }

    void FixedUpdate()
    {
        if (EnemyMeleeMovement.enemyToPlayerDistanceCheckReference < EnemyMeleeMovement.avoidingDistanceFromPlayerReference + 0.1)
        {
            isAbleToAttack = true;
        }
        else
        {
            isAbleToAttack = false;
        }

        if (!isAbleToAttack)
        {
            currentMeleeCooldown = meleeAtkCooldown;
            return;
        }
        
        if (isAbleToAttack)
        {
            if (currentMeleeCooldown > 0 && isAbleToAttack)
            {
                currentMeleeCooldown -= Time.deltaTime;
                return;
            }
            
            EnemyMeleeLoopedAttack();
            currentMeleeCooldown = meleeAtkCooldown;
        }
    }

    private void EnemyMeleeLoopedAttack()
    {
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        Instantiate(meleePrefab, meleePoint.transform.position, meleePoint.transform.rotation);
    }
} 
