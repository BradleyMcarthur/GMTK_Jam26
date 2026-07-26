using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class SpacebarPromptUI : MonoBehaviour
{
    [SerializeField] private float delayBeforePrompt = 10f;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float minAlpha = 0.2f;
    [SerializeField] private float maxAlpha = 1f;
    [SerializeField] private float dismissFadeDuration = 0.3f;

    private CanvasGroup _canvasGroup;
    private LevelTimerManager _timer;
    private Coroutine _pulseRoutine;
    private bool _hasBeenDismissed;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
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

        StartCoroutine(WaitThenPrompt());
    }

    private IEnumerator WaitThenPrompt()
    {
        while (_timer.IsPaused)
            yield return null;

        yield return new WaitForSeconds(delayBeforePrompt);

        if (!_hasBeenDismissed)
            _pulseRoutine = StartCoroutine(Pulse());
    }

    private IEnumerator Pulse()
    {
        while (true)
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f; 
            _canvasGroup.alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
            yield return null;
        }
    }

    private void HandleStateChanged(TimerDirection direction)
    {
        if (_hasBeenDismissed)
            return;

        _hasBeenDismissed = true;

        if (_pulseRoutine != null)
            StopCoroutine(_pulseRoutine);

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float start = _canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < dismissFadeDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(start, 0f, elapsed / dismissFadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = 0f;
    }

    private void OnDestroy()
    {
        if (_timer != null)
            _timer.StateChanged -= HandleStateChanged;
    }
}