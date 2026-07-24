using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    private List<EnemyHp> activeEnemies = new List<EnemyHp>();
    
    public int totalEnemiesKilled = 0;

    void Awake()
    {
        Instance = this;
    }

    public void AddEnemyToManagerList(EnemyHp enemy)
    {
        activeEnemies.Add(enemy);
    }

    public void RemoveDeadEnemyFromList(EnemyHp enemy)
    {
        activeEnemies.Remove(enemy);
    }

    public void EnemyDied(EnemyHp enemy)
    {
        totalEnemiesKilled++;
    }
}
