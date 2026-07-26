using System.Collections;
using UnityEngine;

/// <summary>
/// Plays a fixed sequence of dialogue lines once at the start of the level
/// (e.g. an intro explaining the core mechanic). Queues every line through
/// DialoguePopupUI up front — its own queue handles displaying them one
/// after another, so this script doesn't need to manage timing itself.
/// </summary>
public class IntroDialogue : MonoBehaviour
{
    [SerializeField, TextArea] private string[] lines;
    [SerializeField] private float holdDurationPerLine = 3f;
    [SerializeField] private float startDelay = 0.5f;

    private void Start()
    {
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        // Pause the level timer immediately so it can't tick (or be toggled)
        // while the player is still reading the intro.
        while (LevelTimerManager.Instance == null)
            yield return null;

        LevelTimerManager.Instance.Pause();

        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        // DialoguePopupUI might not have run its Awake() yet if this Start()
        // happens to run first, so wait until it exists rather than assuming.
        while (DialoguePopupUI.Instance == null)
            yield return null;

        foreach (string line in lines)
        {
            DialoguePopupUI.Instance.ShowMessage(line, holdDurationPerLine);
        }

        // Wait for every queued line to actually finish fading in, holding,
        // and fading out — not just for them to be handed off to the queue.
        while (DialoguePopupUI.Instance.IsBusy)
            yield return null;

        LevelTimerManager.Instance.Resume();
    }
}