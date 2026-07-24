using UnityEngine;

/// <summary>
/// Quick test harness for LevelTimerManager — shows the current time on
/// screen and a brief popup whenever the direction swaps
/// </summary>
public class TimerDebugDisplay : MonoBehaviour
{
    [SerializeField] private float popupDuration = 2f;

    private LevelTimerManager _timer;
    private float _popupTimeRemaining;
    private string _popupText = "";

    private void Update()
    {
        if (_timer == null)
        {
            TrySubscribe();
            return;
        }

        if (_popupTimeRemaining > 0f)
            _popupTimeRemaining -= Time.deltaTime;
    }

    private void TrySubscribe()
    {
        if (LevelTimerManager.Instance == null)
            return;

        _timer = LevelTimerManager.Instance;
        _timer.StateChanged += HandleStateChanged;
    }

    private void HandleStateChanged(TimerDirection direction)
    {
        _popupText = direction == TimerDirection.CountingUp ? "COUNTING UP" : "COUNTING DOWN";
        _popupTimeRemaining = popupDuration;
    }

    private void OnDestroy()
    {
        if (_timer != null)
            _timer.StateChanged -= HandleStateChanged;
    }

    private void OnGUI()
    {
        if (_timer == null)
            return;

        GUIStyle timeStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 32,
            alignment = TextAnchor.UpperCenter
        };

        GUI.Label(new Rect(Screen.width / 2f - 100f, 20f, 200f, 50f), _timer.CurrentTime.ToString("F1"), timeStyle);

        if (_popupTimeRemaining > 0f)
        {
            GUIStyle popupStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 48,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };

            GUI.Label(new Rect(Screen.width / 2f - 200f, Screen.height / 2f - 50f, 400f, 100f), _popupText, popupStyle);
        }
    }
}