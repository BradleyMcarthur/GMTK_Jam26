using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlayerLocomotion))]
public class PlayerShooting : MonoBehaviour
{
    private const int DualStreamUnlockLevel = 5;

    [Header("Targeting")]
    [SerializeField] private float range = 8f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Firing")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fallbackFireRate = 2f; // used only if PlayerStats isn't in the scene
    [SerializeField] private bool pauseWhileDashing = true;

    private PlayerLocomotion _locomotion;
    private PlayerProgression _progression;
    private float _fireCooldownRemaining;
    
    private int _streamCount = 1;

    private float FireRate => PlayerStats.Instance != null ? PlayerStats.Instance.FireRate : fallbackFireRate;

    private void Awake()
    {
        _locomotion = GetComponent<PlayerLocomotion>();
    }

    private void Update()
    {
        if (_progression == null)
            TrySubscribeToProgression();

        TickCooldown();

        if (pauseWhileDashing && _locomotion.IsDashing)
            return;

        List<Transform> targets = FindNearestEnemies(_streamCount);
        if (targets.Count == 0)
            return;

        if (_fireCooldownRemaining <= 0f)
            FireAll(targets);
    }

    private void TrySubscribeToProgression()
    {
        if (PlayerProgression.Instance == null)
            return;

        _progression = PlayerProgression.Instance;
        _progression.LeveledUp += HandleLeveledUp;
        
        if (_progression.CurrentLevel >= DualStreamUnlockLevel)
            _streamCount = 2;
    }

    private void HandleLeveledUp(int newLevel)
    {
        if (newLevel >= DualStreamUnlockLevel)
            _streamCount = 2;
    }

    private void TickCooldown()
    {
        if (_fireCooldownRemaining > 0f)
            _fireCooldownRemaining -= Time.deltaTime;
    }
    
    private List<Transform> FindNearestEnemies(int count)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
        if (hits.Length == 0)
            return new List<Transform>();

        return hits
            .OrderBy(hit => ((Vector2)hit.transform.position - (Vector2)transform.position).sqrMagnitude)
            .Take(count)
            .Select(hit => hit.transform)
            .ToList();
    }

    private void FireAll(List<Transform> targets)
    {
        foreach (Transform target in targets)
            Fire(target);
        
        _fireCooldownRemaining = 1f / FireRate;
    }

    private void Fire(Transform target)
    {
        Vector2 direction = (Vector2)target.position - (Vector2)firePoint.position;
        Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        bool isCrit = false;
        float damageMultiplier = 1f;
        if (PlayerStats.Instance != null)
            damageMultiplier = PlayerStats.Instance.RollDamageMultiplier(out isCrit);

        float finalDamage = projectile.BaseDamage * damageMultiplier;

        projectile.Launch(direction, finalDamage, isCrit);
    }

    // Draws the detection range in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    private void OnDestroy()
    {
        if (_progression != null)
            _progression.LeveledUp -= HandleLeveledUp;
    }
}