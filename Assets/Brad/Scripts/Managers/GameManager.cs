using System;
using UnityEngine;

//Why the game ended, for UI/logging to react to differently.
public enum GameOverReason
{
    PlayerDied,
    TimerExpired
}

/// <summary>
/// Central point for game-ending and game-altering conditions. Watches
/// PlayerStats (death) and LevelTimerManager (timer hitting 0 or its max),
/// and fires events for other systems to react to. Singleton, same pattern
/// as the other managers.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsGameOver { get; private set; }

    //Fires once when the game ends, passing the reason
    public event Action<GameOverReason> GameOver;

    //Fires each time the timer reaches its max while counting up for enemy spawners
    public event Action EnemyFrenzyTriggered;

    private PlayerStats _playerStats;
    private LevelTimerManager _timer;

    // Guards against firing EnemyFrenzyTriggered every frame
    private bool _frenzyTriggeredThisCycle;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (_playerStats == null)
            TrySubscribeToPlayerStats();

        if (_timer == null)
            TrySubscribeToTimer();
    }

    private void TrySubscribeToPlayerStats()
    {
        if (PlayerStats.Instance == null)
            return;

        _playerStats = PlayerStats.Instance;
        _playerStats.Died += HandlePlayerDied;
    }

    private void TrySubscribeToTimer()
    {
        if (LevelTimerManager.Instance == null)
            return;

        _timer = LevelTimerManager.Instance;
        _timer.TimeChanged += HandleTimeChanged;
        _timer.StateChanged += HandleStateChanged;
    }

    private void HandlePlayerDied()
    {
        TriggerGameOver(GameOverReason.PlayerDied);
    }

    private void HandleTimeChanged(float currentTime)
    {
        if (IsGameOver)
            return;

        if (_timer.CurrentDirection == TimerDirection.CountingDown && currentTime <= 0f)
        {
            TriggerGameOver(GameOverReason.TimerExpired);
            return;
        }

        if (!_frenzyTriggeredThisCycle
            && _timer.CurrentDirection == TimerDirection.CountingUp
            && currentTime >= _timer.MaxTime)
        {
            _frenzyTriggeredThisCycle = true;
            EnemyFrenzyTriggered?.Invoke();
        }
    }

    private void HandleStateChanged(TimerDirection direction)
    {
        // Re-arm the frenzy trigger once the timer starts counting down again
        if (direction == TimerDirection.CountingDown)
            _frenzyTriggeredThisCycle = false;
    }

    private void TriggerGameOver(GameOverReason reason)
    {
        if (IsGameOver)
            return;

        IsGameOver = true;
        GameOver?.Invoke(reason);
    }

    private void OnDestroy()
    {
        if (_playerStats != null)
            _playerStats.Died -= HandlePlayerDied;

        if (_timer != null)
        {
            _timer.TimeChanged -= HandleTimeChanged;
            _timer.StateChanged -= HandleStateChanged;
        }
    }
}