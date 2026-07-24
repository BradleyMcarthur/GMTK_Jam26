using UnityEngine;

public class EnemyRangedMovement : MonoBehaviour
{
    public Rigidbody2D rangedRb;
    public Transform playerTransform;

    public float enemyCurrentMoveSpeed;
    public float avoidingDistanceFromPlayer = 5f;

    [SerializeField] private float enemyToPlayerDistanceCheck;
    
    void Start()
    {
        if (!playerTransform)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (!rangedRb)
        {
            rangedRb = GetComponent<Rigidbody2D>();
        }
    }
    
    void FixedUpdate()
    {
        Vector3 directionToPlayer = playerTransform.position - gameObject.transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        enemyToPlayerDistanceCheck = distanceToPlayer;
        
        if (distanceToPlayer > avoidingDistanceFromPlayer)
        {
            MovingTowardsPlayer(directionToPlayer);
        }
    }

    public void MovingTowardsPlayer(Vector3 directionToPlayer)
    {
        Vector2 newPosition = rangedRb.transform.position + (directionToPlayer * (enemyCurrentMoveSpeed * Time.fixedDeltaTime));
        rangedRb.MovePosition(newPosition);
    }
}
