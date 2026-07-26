using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FrenzyModeUI : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.2f;

    private CanvasGroup _canvasGroup;
    private GameManager _gameManager;
    private LevelTimerManager _timer;
    private Coroutine _fadeRoutine;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
    }

    private void Update()
    {
        if (_gameManager == null)
            TrySubscribeToGameManager();

        if (_timer == null)
            TrySubscribeToTimer();
    }

    private void TrySubscribeToGameManager()
    {
        if (GameManager.Instance == null)
            return;

        _gameManager = GameManager.Instance;
        _gameManager.EnemyFrenzyTriggered += HandleFrenzyTriggered;
    }

    private void TrySubscribeToTimer()
    {
        if (LevelTimerManager.Instance == null)
            return;

        _timer = LevelTimerManager.Instance;
        _timer.StateChanged += HandleStateChanged;
    }

    private void HandleFrenzyTriggered()
    {
        SetVisible(true);
    }

    private void HandleStateChanged(TimerDirection direction)
    {
        // Frenzy always ends when the player swaps back to counting down
        if (direction == TimerDirection.CountingDown)
            SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _fadeRoutine = StartCoroutine(Fade(visible ? 1f : 0f));
    }

    private IEnumerator Fade(float target)
    {
        float start = _canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(start, target, elapsed / fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = target;
        _fadeRoutine = null;
    }

    private void OnDestroy()
    {
        if (_gameManager != null)
            _gameManager.EnemyFrenzyTriggered -= HandleFrenzyTriggered;

        if (_timer != null)
            _timer.StateChanged -= HandleStateChanged;
    }
}
