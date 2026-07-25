using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInputReader))]
public class PlayerLocomotion : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float rotationSpeed = 720f; // degrees per second
    [SerializeField] private float fallbackMoveSpeed = 6f; // used only if PlayerStats isn't in the scene

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float fallbackDashCooldown = 0.8f; // used only if PlayerStats isn't in the scene

    /// <summary>Raised the instant a dash begins.</summary>
    public event Action DashStarted;

    /// <summary>Raised the instant a dash ends.</summary>
    public event Action DashEnded;

    public bool IsDashing { get; private set; }
    public Vector2 FacingDirection => _lastMoveDirection;
    public bool IsMoving => _moveDirection.sqrMagnitude > 0.01f;
    
    private float MoveSpeed => PlayerStats.Instance != null ? PlayerStats.Instance.MoveSpeed : fallbackMoveSpeed;
    private float DashCooldownValue => PlayerStats.Instance != null ? PlayerStats.Instance.DashCooldown : fallbackDashCooldown;

    private Rigidbody2D _rb;
    private PlayerInputReader _input;

    private Vector2 _moveDirection;
    private Vector2 _lastMoveDirection = Vector2.up; // assumes sprite's default art faces "up"

    private float _dashTimeRemaining;
    private float _dashCooldownRemaining;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _input = GetComponent<PlayerInputReader>();
        
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void Update()
    {
        ReadMovementInput();
        HandleDashInput();
        TickDashTimers();
    }

    private void FixedUpdate()
    {
        ApplyMotion();
        //RotateTowardsFacing();
    }

    private void ReadMovementInput()
    {
        Vector2 raw = _input.MoveInput;
        _moveDirection = raw.sqrMagnitude > 1f ? raw.normalized : raw;

        if (IsMoving)
            _lastMoveDirection = _moveDirection;
    }

    private void HandleDashInput()
    {
        bool canDash = !IsDashing && _dashCooldownRemaining <= 0f;
        if (_input.DashPressed && canDash)
            StartDash();
    }

    private void StartDash()
    {
        IsDashing = true;
        _dashTimeRemaining = dashDuration;
        _dashCooldownRemaining = DashCooldownValue;
        DashStarted?.Invoke();
    }

    private void TickDashTimers()
    {
        if (_dashCooldownRemaining > 0f)
            _dashCooldownRemaining -= Time.deltaTime;

        if (!IsDashing)
            return;

        _dashTimeRemaining -= Time.deltaTime;
        if (_dashTimeRemaining <= 0f)
        {
            IsDashing = false;
            DashEnded?.Invoke();
        }
    }

    private void ApplyMotion()
    {
        Vector2 velocity = IsDashing
            ? _lastMoveDirection * dashSpeed
            : _moveDirection * MoveSpeed;
        
        _rb.linearVelocity = velocity;
    }

    private void RotateTowardsFacing()
    {
        if (_lastMoveDirection.sqrMagnitude < 0.01f)
            return;

        // -90 offset assumes the sprite's "forward" is drawn facing up.
        float targetAngle = Mathf.Atan2(_lastMoveDirection.y, _lastMoveDirection.x) * Mathf.Rad2Deg - 90f;
        float newAngle = Mathf.MoveTowardsAngle(_rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        _rb.MoveRotation(newAngle);
    }
}