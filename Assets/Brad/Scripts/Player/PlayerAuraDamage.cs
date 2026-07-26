using UnityEngine;

public class PlayerAuraDamage : MonoBehaviour
{
    private const int AuraUnlockLevel = 10;
    
    [SerializeField] private GameObject AuraVisual;

    [Header("Aura")]
    [SerializeField] private float radius = 3f;
    [SerializeField] private float damagePerTick = 5f;
    [SerializeField] private float tickInterval = 0.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Color gizmoColor = new Color(1f, 0.5f, 0f, 0.3f);

    private PlayerProgression _progression;
    private bool _isUnlocked;
    private float _tickCooldownRemaining;
    
    public float Radius => radius;
    
    public bool IsUnlocked => _isUnlocked;

    private void Update()
    {
        if (_progression == null)
        {
            TrySubscribe();
            return;
        }

        if (!_isUnlocked)
            return;

        _tickCooldownRemaining -= Time.deltaTime;
        if (_tickCooldownRemaining <= 0f)
        {
            DamageEnemiesInRange();
            _tickCooldownRemaining = tickInterval;
        }
    }

    private void TrySubscribe()
    {
        if (PlayerProgression.Instance == null)
            return;

        _progression = PlayerProgression.Instance;
        _progression.LeveledUp += HandleLeveledUp;
        
        _isUnlocked = _progression.CurrentLevel >= AuraUnlockLevel;
    }

    private void HandleLeveledUp(int newLevel)
    {
        if (newLevel >= AuraUnlockLevel)
        {
            _isUnlocked = true;
            AuraVisual.SetActive(true);
        }
    }

    private void DamageEnemiesInRange()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(damagePerTick);
                DamageNumberSpawner.Instance?.SpawnDamageNumber(hit.transform.position, damagePerTick, false);
            }
        }
    }

    private void OnDestroy()
    {
        if (_progression != null)
            _progression.LeveledUp -= HandleLeveledUp;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}