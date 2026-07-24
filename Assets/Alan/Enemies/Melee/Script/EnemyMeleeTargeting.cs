using UnityEngine;

public class EnemyMeleeTargeting : MonoBehaviour
{
    public Transform playerTransform;
    
    void Start()
    {
        if (!playerTransform)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
    
    void FixedUpdate()
    {
        Vector3 directionToPlayer = playerTransform.position - gameObject.transform.position;
        
        RotatingEnemyTowardsPlayer(directionToPlayer);
    }

    private void RotatingEnemyTowardsPlayer(Vector3 directionToPlayer)
    {
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
