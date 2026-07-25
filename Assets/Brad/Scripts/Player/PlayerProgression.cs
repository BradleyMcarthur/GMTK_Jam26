using System;
using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    public static PlayerProgression Instance { get; private set; }

    [Header("Level Thresholds")]
    [SerializeField] private float baseThreshold = 100f;
    [SerializeField] private float thresholdGrowth = 1.25f; // multiplier applied to the threshold each level

    [Header("Flat Buffs Per Level")]
    [SerializeField] private float moveSpeedBuff = 0.5f;
    [SerializeField] private float fireRateBuff = 0.2f;
    [SerializeField] private float critChanceBuff = 0.02f;
    [SerializeField] private float critDamageBuff = 0.1f;
    [SerializeField] private float maxHealthBuff = 10f;

    public int CurrentLevel { get; private set; } = 1;
    public float CurrentResource { get; private set; }
    public float ThresholdForNextLevel { get; private set; }

    /// <summary>True only while the level timer is counting down — resources can't be collected or spent while counting up.</summary>
    public bool CanCollectResources { get; private set; }

    /// <summary>Fires whenever the resource changes, passing (current, thresholdForNextLevel) — for a progress bar.</summary>
    public event Action<float, float> ResourceChanged;

    /// <summary>Fires whenever a level-up happens, passing the new level — for a popup/flash/SFX.</summary>
    public event Action<int> LeveledUp;

    private LevelTimerManager _timer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        ThresholdForNextLevel = baseThreshold;
    }

    private void Update()
    {
        if (_timer == null)
            TrySubscribe();
    }

    private void TrySubscribe()
    {
        if (LevelTimerManager.Instance == null)
            return;

        _timer = LevelTimerManager.Instance;
        _timer.StateChanged += HandleStateChanged;
        
        CanCollectResources = _timer.CurrentDirection == TimerDirection.CountingDown;
    }

    private void HandleStateChanged(TimerDirection direction)
    {
        CanCollectResources = direction == TimerDirection.CountingDown;
    }

    private void OnDestroy()
    {
        if (_timer != null)
            _timer.StateChanged -= HandleStateChanged;
    }

    public void AddResource(float amount)
    {
        // Single source of truth for the counting-down-only rule — any future
        // resource source (shop, quest reward, etc.) automatically respects it
        // without needing to check the timer itself.
        if (!CanCollectResources)
            return;

        CurrentResource += amount;

        // A loop rather than a single check, in case one big pickup crosses
        // more than one threshold at once.
        while (CurrentResource >= ThresholdForNextLevel)
            LevelUp();

        ResourceChanged?.Invoke(CurrentResource, ThresholdForNextLevel);
    }

    private void LevelUp()
    {
        CurrentResource -= ThresholdForNextLevel;
        CurrentLevel++;
        ThresholdForNextLevel *= thresholdGrowth;

        ApplyLevelUpBuffs();
        LeveledUp?.Invoke(CurrentLevel);
    }

    private void ApplyLevelUpBuffs()
    {
        if (PlayerStats.Instance == null)
            return;

        PlayerStats.Instance.AddMoveSpeed(moveSpeedBuff);
        PlayerStats.Instance.AddFireRate(fireRateBuff);
        PlayerStats.Instance.AddCritChance(critChanceBuff);
        PlayerStats.Instance.AddCritDamageMultiplier(critDamageBuff);
        PlayerStats.Instance.AddMaxHealth(maxHealthBuff);
    }
}
