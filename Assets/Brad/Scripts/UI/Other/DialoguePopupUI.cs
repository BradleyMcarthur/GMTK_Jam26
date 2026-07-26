using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class DialoguePopupUI : MonoBehaviour
{
    public static DialoguePopupUI Instance { get; private set; }

    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float defaultHoldDuration = 2.5f;

    private CanvasGroup _canvasGroup;
    private readonly Queue<QueuedMessage> _queue = new Queue<QueuedMessage>();
    private bool _isShowing;
    
    public bool IsBusy => _isShowing;

    private struct QueuedMessage
    {
        public string Text;
        public float HoldDuration;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
    }
    
    public void ShowMessage(string text, float holdDuration = -1f)
    {
        _queue.Enqueue(new QueuedMessage
        {
            Text = text,
            HoldDuration = holdDuration >= 0f ? holdDuration : defaultHoldDuration
        });

        if (!_isShowing)
            StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        _isShowing = true;

        while (_queue.Count > 0)
        {
            QueuedMessage message = _queue.Dequeue();

            if (messageText != null)
                messageText.text = message.Text;

            yield return Fade(0f, 1f, fadeDuration);
            yield return new WaitForSeconds(message.HoldDuration);
            yield return Fade(1f, 0f, fadeDuration);
        }

        _isShowing = false;
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
}