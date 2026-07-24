using UnityEngine;

/// <summary>
/// A simple straight-line projectile. Launched with a direction and speed,
/// deals damage to the first IDamageable it touches, then destroys itself.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float damage = 10f;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    //Sets the projectile flying in the given direction and orients it to match
    public void Launch(Vector2 direction, float damageOverride = -1f)
    {
        if (damageOverride >= 0f)
            damage = damageOverride;

        direction = direction.normalized;
        _rb.linearVelocity = direction * speed;

        // -90 assumes sprite art faces "up" by default, matching PlayerLocomotion
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
