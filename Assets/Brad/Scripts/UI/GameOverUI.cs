using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TMP_Text reasonText;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private bool pauseGameOnShow = true;

    private CanvasGroup _canvasGroup;
    private GameManager _gameManager;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    private void Update()
    {
        // GameManager might initialize on a different frame, so subscribe lazily.
        if (_gameManager == null)
            TrySubscribe();
    }

    private void TrySubscribe()
    {
        if (GameManager.Instance == null)
            return;

        _gameManager = GameManager.Instance;
        _gameManager.GameOver += HandleGameOver;
    }

    private void HandleGameOver(GameOverReason reason)
    {
        if (reasonText != null)
            reasonText.text = reason == GameOverReason.PlayerDied ? "YOU DIED" : "TIME'S UP";

        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        StartCoroutine(FadeIn());

        if (pauseGameOnShow)
            Time.timeScale = 0f;
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;

        // Unscaled time — Time.timeScale is likely already 0 by the time this
        // runs, and the fade should still play rather than freeze on frame one.
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = 1f;
    }

    //Restart button OnClick
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
    
    //Quit Button
    

    private void OnDestroy()
    {
        if (_gameManager != null)
            _gameManager.GameOver -= HandleGameOver;
    }
}
