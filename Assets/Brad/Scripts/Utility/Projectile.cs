using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float damage = 10f;
    
    public float BaseDamage => damage;

    private bool _isCrit;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
    
    public void Launch(Vector2 direction, float damageOverride = -1f, bool isCrit = false)
    {
        if (damageOverride >= 0f)
            damage = damageOverride;

        _isCrit = isCrit;

        direction = direction.normalized;
        _rb.linearVelocity = direction * speed;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(damage);
            DamageNumberSpawner.Instance?.SpawnDamageNumber(transform.position, damage, _isCrit);
            Destroy(gameObject);
        }
    }
}