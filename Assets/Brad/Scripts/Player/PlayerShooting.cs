using UnityEngine;

/// <summary>
/// Auto-fires at the nearest enemy in range, on a timer. Classic auto-shooter
/// behavior — the player never aims manually, just moves and dashes while this handles combat.
/// </summary>

[RequireComponent(typeof(PlayerLocomotion))]
public class PlayerShooting : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private float range = 8f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Firing")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 2f; // shots per second
    [SerializeField] private bool pauseWhileDashing = true;

    private PlayerLocomotion _locomotion;
    private float _fireCooldownRemaining;

    private void Awake()
    {
        _locomotion = GetComponent<PlayerLocomotion>();
    }

    private void Update()
    {
        TickCooldown();

        if (pauseWhileDashing && _locomotion.IsDashing)
            return;

        Transform target = FindNearestEnemy();
        if (target == null)
            return;

        if (_fireCooldownRemaining <= 0f)
            Fire(target);
    }

    private void TickCooldown()
    {
        if (_fireCooldownRemaining > 0f)
            _fireCooldownRemaining -= Time.deltaTime;
    }

    private Transform FindNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
        if (hits.Length == 0)
            return null;

        Transform nearest = null;
        float nearestSqrDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            float sqrDistance = ((Vector2)hit.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (sqrDistance < nearestSqrDistance)
            {
                nearestSqrDistance = sqrDistance;
                nearest = hit.transform;
            }
        }

        return nearest;
    }

    private void Fire(Transform target)
    {
        Vector2 direction = (Vector2)target.position - (Vector2)firePoint.position;
        Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.Launch(direction);

        _fireCooldownRemaining = 1f / fireRate;
    }

    // Draws the detection range in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
