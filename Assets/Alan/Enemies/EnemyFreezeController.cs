using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyFreezeController : MonoBehaviour
{
    private LevelTimerManager _timer;
    private Rigidbody2D _rb;
    private MonoBehaviour[] _behavioursToFreeze;

    private Vector2 _velocityBeforeFreeze;
    private bool _isFrozen;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        _behavioursToFreeze = GetComponents<MonoBehaviour>().Where(b => b != this && b is not IDamageable).ToArray();
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

        // Sync to whatever state the timer is already in, rather than assuming.
        SetFrozen(_timer.CurrentDirection == TimerDirection.CountingDown);
    }

    private void HandleStateChanged(TimerDirection direction)
    {
        SetFrozen(direction == TimerDirection.CountingDown);
    }

    private void SetFrozen(bool freeze)
    {
        if (freeze == _isFrozen)
            return;

        _isFrozen = freeze;

        foreach (MonoBehaviour behaviour in _behavioursToFreeze)
        {
            if (behaviour != null)
                behaviour.enabled = !freeze;
        }

        if (freeze)
        {
            // Remember current velocity and stop dead, rather than disabling
            _velocityBeforeFreeze = _rb.linearVelocity;
            _rb.linearVelocity = Vector2.zero;
        }
        else
        {
            _rb.linearVelocity = _velocityBeforeFreeze;
        }
    }

    private void OnDestroy()
    {
        if (_timer != null)
            _timer.StateChanged -= HandleStateChanged;
    }
}