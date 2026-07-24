using System;
using UnityEngine;

public class EnemyMeleeMovement : MonoBehaviour
{
    public Rigidbody2D meleeRb;
    public Transform playerTransform;
    public static float enemyToPlayerDistanceCheckReference;
    public static float avoidingDistanceFromPlayerReference;

    public float enemyCurrentMoveSpeed;
    public float avoidingDistanceFromPlayer;
    
    [SerializeField] private float enemyToPlayerDistanceCheck;

    private void Awake()
    {
        avoidingDistanceFromPlayerReference = avoidingDistanceFromPlayer;
    }

    void Start()
    {
        if (!playerTransform)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (!meleeRb)
        {
            meleeRb = GetComponent<Rigidbody2D>();
        }
    }
    
    void FixedUpdate()
    {
        Vector3 directionToPlayer = playerTransform.position - gameObject.transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        enemyToPlayerDistanceCheck = distanceToPlayer;
        enemyToPlayerDistanceCheckReference = distanceToPlayer;
        
        if (distanceToPlayer > avoidingDistanceFromPlayer)
        {
            MovingTowardsPlayer(directionToPlayer);
        }
    }

    private void MovingTowardsPlayer(Vector3 directionToPlayer)
    {
        Vector2 newPosition = meleeRb.transform.position + (directionToPlayer * (enemyCurrentMoveSpeed * Time.fixedDeltaTime));
        meleeRb.MovePosition(newPosition);
    }
}
