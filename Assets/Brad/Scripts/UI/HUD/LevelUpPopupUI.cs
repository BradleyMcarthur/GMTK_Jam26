using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class LevelUpPopupUI : MonoBehaviour
{
    [SerializeField] private TMP_Text levelUpText;
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float holdDuration = 1.5f;

    private CanvasGroup _canvasGroup;
    private PlayerProgression _progression;
    private Coroutine _popupRoutine;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
    }

    private void Update()
    {
        if (_progression == null)
            TrySubscribe();
    }

    private void TrySubscribe()
    {
        if (PlayerProgression.Instance == null)
            return;

        _progression = PlayerProgression.Instance;
        _progression.LeveledUp += HandleLeveledUp;
    }

    private void HandleLeveledUp(int newLevel)
    {
        if (levelUpText != null)
            levelUpText.text = $"LEVEL UP!\nLv. {newLevel}";
        
        if (_popupRoutine != null)
            StopCoroutine(_popupRoutine);

        _popupRoutine = StartCoroutine(PlayPopup());
    }

    private IEnumerator PlayPopup()
    {
        yield return Fade(0f, 1f, fadeDuration);
        yield return new WaitForSeconds(holdDuration);
        yield return Fade(1f, 0f, fadeDuration);

        _popupRoutine = null;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        _canvasGroup.alpha = to;
    }

    private void OnDestroy()
    {
        if (_progression != null)
            _progression.LeveledUp -= HandleLeveledUp;
    }
}
