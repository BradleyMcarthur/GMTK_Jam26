using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;    // Image Type: Filled, Fill Method: Horizontal
    [SerializeField] private TMP_Text valueText; // optional, shows e.g. "82 / 100"

    private PlayerStats _playerStats;

    private void Update()
    {
        // PlayerStats might initialize on a different frame, so subscribe lazily.
        if (_playerStats == null)
            TrySubscribe();
    }

    private void TrySubscribe()
    {
        if (PlayerStats.Instance == null)
            return;

        _playerStats = PlayerStats.Instance;
        _playerStats.HealthChanged += UpdateDisplay;

        UpdateDisplay(_playerStats.CurrentHealth, _playerStats.MaxHealth);
    }

    private void UpdateDisplay(float current, float max)
    {
        if (fillImage != null)
            fillImage.fillAmount = max > 0f ? current / max : 0f;

        if (valueText != null)
            valueText.text = $"{current:F0} / {max:F0}";
    }

    private void OnDestroy()
    {
        if (_playerStats != null)
            _playerStats.HealthChanged -= UpdateDisplay;
    }
}