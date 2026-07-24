using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Positioning")]
    [SerializeField] private Vector2 offset = Vector2.zero;
    [SerializeField] private float fixedZ = -10f; // camera depth; keep negative so the scene renders in front of it
    [SerializeField] private float followSmoothTime = 0.15f;

    [Header("Look-Ahead (optional)")]
    [SerializeField] private bool useLookAhead = false;
    [SerializeField] private float lookAheadDistance = 2f;
    [SerializeField] private float lookAheadSmoothTime = 0.3f;

    private Vector2 _followVelocity;
    private Vector2 _lookAheadVelocity;
    private Vector2 _currentLookAhead;
    private Vector2 _lastTargetPosition;

    private void Start()
    {
        if (target != null)
            _lastTargetPosition = target.position;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        UpdateLookAhead();

        Vector2 desiredPosition = (Vector2)target.position + offset + _currentLookAhead;
        Vector2 smoothedPosition = Vector2.SmoothDamp(
            transform.position,
            desiredPosition,
            ref _followVelocity,
            followSmoothTime);

        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, fixedZ);
    }

    private void UpdateLookAhead()
    {
        if (!useLookAhead)
        {
            _currentLookAhead = Vector2.zero;
            return;
        }

        Vector2 targetMovement = (Vector2)target.position - _lastTargetPosition;
        Vector2 desiredLookAhead = targetMovement.normalized * lookAheadDistance;

        _currentLookAhead = Vector2.SmoothDamp(
            _currentLookAhead,
            desiredLookAhead,
            ref _lookAheadVelocity,
            lookAheadSmoothTime);

        _lastTargetPosition = target.position;
    }

    /// <summary>Call this if the target is assigned or swapped at runtime.</summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        _lastTargetPosition = target.position;
    }
}