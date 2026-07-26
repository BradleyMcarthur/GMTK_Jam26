using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum TimerDirection
{
    CountingDown,
    CountingUp
}
public class LevelTimerManager : MonoBehaviour
{
    public static LevelTimerManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float startingTime = 60f;

    [Header("Input")]
    [SerializeField] private string toggleBinding = "<Keyboard>/space";
    [SerializeField] private float toggleCooldown = 0.5f;

    //Fires whenever the timer's direction is swapped, passing the new direction
    public event Action<TimerDirection> StateChanged;

    //Fires every frame with the updated time value — hook this up to UI text
    public event Action<float> TimeChanged;

    public float CurrentTime { get; private set; }
    public TimerDirection CurrentDirection { get; private set; } = TimerDirection.CountingDown;

    //The timer's starting value, and the cap CurrentTime won't exceed while counting up
    public float MaxTime => startingTime;

    //While true, the timer doesn't tick and the toggle key does nothing — e.g. during an intro sequence
    public bool IsPaused { get; private set; }

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
        if (IsPaused)
            return;

        if (_toggleCooldownRemaining > 0f)
            _toggleCooldownRemaining -= Time.deltaTime;

        if (_toggleAction.WasPressedThisFrame() && _toggleCooldownRemaining <= 0f)
            ToggleDirection();

        TickTimer();
    }
    
    public void Pause() => IsPaused = true;
    
    public void Resume() => IsPaused = false;

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