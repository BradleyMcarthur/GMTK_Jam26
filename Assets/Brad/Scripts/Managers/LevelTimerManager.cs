using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Which way the level timer is currently running.</summary>
public enum TimerDirection
{
    CountingDown,
    CountingUp
}

/// <summary>
/// Tracks a single global level timer. Starts counting down from 60, the player can toggle it to count up instead (and back again)
/// at any time with a key press. Other systems react via events
/// </summary>
public class LevelTimerManager : MonoBehaviour
{
    /// <summary>Global access point so other scripts don't need a direct reference.</summary>
    public static LevelTimerManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float startingTime = 60f;

    [Header("Input")]
    [SerializeField] private string toggleBinding = "<Keyboard>/space";
    [SerializeField] private float toggleCooldown = 0.5f;

    /// <summary>Fires whenever the timer's direction is swapped, passing the new direction.</summary>
    public event Action<TimerDirection> StateChanged;

    /// <summary>Fires every frame with the updated time value for UI text.</summary>
    public event Action<float> TimeChanged;

    public float CurrentTime { get; private set; }
    public TimerDirection CurrentDirection { get; private set; } = TimerDirection.CountingDown;

    private InputAction _toggleAction;
    private float _toggleCooldownRemaining;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        CurrentTime = startingTime;
        _toggleAction = new InputAction("ToggleTimerDirection", binding: toggleBinding);
    }

    private void OnEnable()
    {
        _toggleAction.Enable();
    }

    private void OnDisable()
    {
        _toggleAction.Disable();
    }

    private void Update()
    {
        if (_toggleCooldownRemaining > 0f)
            _toggleCooldownRemaining -= Time.deltaTime;

        if (_toggleAction.WasPressedThisFrame() && _toggleCooldownRemaining <= 0f)
            ToggleDirection();

        TickTimer();
    }

    private void TickTimer()
    {
        float delta = CurrentDirection == TimerDirection.CountingDown ? -Time.deltaTime : Time.deltaTime;
        CurrentTime += delta;

        if (CurrentDirection == TimerDirection.CountingDown && CurrentTime < 0f)
            CurrentTime = 0f;
        else if (CurrentDirection == TimerDirection.CountingUp && CurrentTime > startingTime)
            CurrentTime = startingTime;

        TimeChanged?.Invoke(CurrentTime);
    }

    private void ToggleDirection()
    {
        CurrentDirection = CurrentDirection == TimerDirection.CountingDown
            ? TimerDirection.CountingUp
            : TimerDirection.CountingDown;

        _toggleCooldownRemaining = toggleCooldown;
        StateChanged?.Invoke(CurrentDirection);
    }
    
    public void ResetTimer()
    {
        CurrentTime = startingTime;
        CurrentDirection = TimerDirection.CountingDown;
        StateChanged?.Invoke(CurrentDirection);
        TimeChanged?.Invoke(CurrentTime);
    }

    private void OnDestroy()
    {
        _toggleAction.Dispose();
    }
}