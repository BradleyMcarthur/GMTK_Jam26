using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    public static PlayerStats Instance { get; private set; }

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float dashCooldown = 0.8f;

    [Header("Combat")]
    [SerializeField] private float fireRate = 2f; // shots per second
    [SerializeField, Range(0f, 1f)] private float critChance = 0.1f;
    [SerializeField] private float critDamageMultiplier = 2f;

    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    public float MoveSpeed => moveSpeed;
    public float DashCooldown => dashCooldown;
    public float FireRate => fireRate;
    public float CritChance => critChance;
    public float CritDamageMultiplier => critDamageMultiplier;

    /// <summary>Fires whenever health changes, passing (current, max).</summary>
    public event Action<float, float> HealthChanged;

    /// <summary>Fires once when health hits zero.</summary>
    public event Action Died;

    /// <summary>Fires whenever a crit roll succeeds.</summary>
    public event Action CritOccurred;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        HealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0f)
        {
            IsDead = true;
            Died?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        HealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    /// <summary>
    /// Rolls a crit check against CritChance. Returns the damage multiplier
    /// to apply (1x on a normal hit, CritDamageMultiplier on a crit).
    /// </summary>
    public float RollDamageMultiplier(out bool isCrit)
    {
        isCrit = UnityEngine.Random.value <= critChance;
        if (isCrit)
            CritOccurred?.Invoke();

        return isCrit ? critDamageMultiplier : 1f;
    }
    
    // Flat add versions, for leveling up that buff on top of whatever the current value already is rather than overwriting it.
    public void AddMoveSpeed(float amount) => moveSpeed += amount;
    public void AddFireRate(float amount) => fireRate += amount;
    public void AddCritChance(float amount) => critChance = Mathf.Clamp01(critChance + amount);
    public void AddCritDamageMultiplier(float amount) => critDamageMultiplier += amount;
 
    //Increases max health and heals by the same amount, so the buff isn't wasted
    public void AddMaxHealth(float amount)
    {
        maxHealth += amount;
        CurrentHealth += amount;
        HealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
 
    //Reduces dash cooldown by a flat amount, floored so it never hits zero or negative
    public void ReduceDashCooldown(float amount) => dashCooldown = Mathf.Max(0.05f, dashCooldown - amount);
}
