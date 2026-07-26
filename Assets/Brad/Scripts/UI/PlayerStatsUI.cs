using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text moveSpeedText;
    [SerializeField] private TMP_Text fireRateText;
    [SerializeField] private TMP_Text critChanceText;
    [SerializeField] private TMP_Text critDamageText;

    private PlayerStats _playerStats;
    private PlayerProgression _progression;

    private void Update()
    {
        if (_playerStats == null)
            TrySubscribeToStats();

        if (_progression == null)
            TrySubscribeToProgression();
    }

    private void TrySubscribeToStats()
    {
        if (PlayerStats.Instance == null)
            return;

        _playerStats = PlayerStats.Instance;
        RefreshStatsText();
    }

    private void TrySubscribeToProgression()
    {
        if (PlayerProgression.Instance == null)
            return;

        _progression = PlayerProgression.Instance;
        _progression.LeveledUp += HandleLeveledUp;

        UpdateLevelText(_progression.CurrentLevel);
    }

    private void HandleLeveledUp(int newLevel)
    {
        UpdateLevelText(newLevel);
        RefreshStatsText(); // stat values just changed, pull fresh numbers
    }

    private void UpdateLevelText(int level)
    {
        if (levelText != null)
            levelText.text = $"Lv. {level}";
    }

    public void RefreshStatsText()
    {
        if (_playerStats == null)
            return;

        if (moveSpeedText != null) moveSpeedText.text = $"Speed: {_playerStats.MoveSpeed:F1}";
        if (fireRateText != null) fireRateText.text = $"Fire Rate: {_playerStats.FireRate:F1}/s";
        if (critChanceText != null) critChanceText.text = $"Crit: {_playerStats.CritChance * 100f:F0}%";
        if (critDamageText != null) critDamageText.text = $"Crit Dmg: x{_playerStats.CritDamageMultiplier:F1}";
    }

    private void OnDestroy()
    {
        if (_progression != null)
            _progression.LeveledUp -= HandleLeveledUp;
    }
}
