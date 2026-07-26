using UnityEngine;

public class ProgressionDebugDisplay : MonoBehaviour
{
    [SerializeField] private float popupDuration = 2f;

    private PlayerProgression _progression;
    private float _popupTimeRemaining;
    private string _popupText = "";

    private void Update()
    {
        if (_progression == null)
        {
            TrySubscribe();
            return;
        }

        if (_popupTimeRemaining > 0f)
            _popupTimeRemaining -= Time.deltaTime;
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
        _popupText = $"LEVEL UP! ({newLevel})";
        _popupTimeRemaining = popupDuration;
    }

    private void OnDestroy()
    {
        if (_progression != null)
            _progression.LeveledUp -= HandleLeveledUp;
    }

    private void OnGUI()
    {
        if (_progression == null)
            return;

        GUIStyle infoStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 24,
            alignment = TextAnchor.UpperLeft
        };

        string collectStatus = _progression.CanCollectResources ? "COLLECTING" : "LOCKED";
        string infoText = $"Level {_progression.CurrentLevel}\n" +
                           $"{_progression.CurrentResource:F0} / {_progression.ThresholdForNextLevel:F0}\n" +
                           $"[{collectStatus}]";

        GUI.Label(new Rect(20f, 20f, 250f, 100f), infoText, infoStyle);

        if (_popupTimeRemaining > 0f)
        {
            GUIStyle popupStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 48,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };

            GUI.Label(new Rect(Screen.width / 2f - 200f, Screen.height / 2f - 120f, 400f, 100f), _popupText, popupStyle);
        }
    }
}
