using System;
using UnityEngine;

public class KillCounter : MonoBehaviour
{
    public static KillCounter Instance { get; private set; }

    public int CurrentKills { get; private set; }
    
    public event Action<int> KillCountChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterKill()
    {
        CurrentKills++;
        KillCountChanged?.Invoke(CurrentKills);
    }
    
    public void ResetCount()
    {
        CurrentKills = 0;
        KillCountChanged?.Invoke(CurrentKills);
    }
}
