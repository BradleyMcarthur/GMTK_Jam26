using UnityEngine;
using TMPro;

public class KillCounterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text killCountText;
    [SerializeField] private string prefix = "Kills: ";

    private KillCounter _killCounter;

    private void Update()
    {
        if (_killCounter == null)
            TrySubscribe();
    }

    private void TrySubscribe()
    {
        if (KillCounter.Instance == null)
            return;

        _killCounter = KillCounter.Instance;
        _killCounter.KillCountChanged += UpdateDisplay;

        UpdateDisplay(_killCounter.CurrentKills);
    }

    private void UpdateDisplay(int count)
    {
        if (killCountText != null)
            killCountText.text = $"{prefix}{count}";
    }

    private void OnDestroy()
    {
        if (_killCounter != null)
            _killCounter.KillCountChanged -= UpdateDisplay;
    }
}
