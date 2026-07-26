using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Color countingDownColor = Color.cyan;
    [SerializeField] private Color countingUpColor = Color.red;

    private LevelTimerManager _timer;

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
        _timer.TimeChanged += UpdateTimeText;
        _timer.StateChanged += UpdateColor;

        UpdateTimeText(_timer.CurrentTime);
        UpdateColor(_timer.CurrentDirection);
    }

    private void UpdateTimeText(float time)
    {
        if (timeText == null)
            return;

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timeText.text = $"{minutes:00}:{seconds:00}";
    }

    private void UpdateColor(TimerDirection direction)
    {
        if (timeText == null)
            return;

        timeText.color = direction == TimerDirection.CountingDown ? countingDownColor : countingUpColor;
    }

    private void OnDestroy()
    {
        if (_timer != null)
        {
            _timer.TimeChanged -= UpdateTimeText;
            _timer.StateChanged -= UpdateColor;
        }
    }
}
