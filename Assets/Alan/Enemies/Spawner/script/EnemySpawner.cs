using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    private LevelTimerManager _timer;
    
    [SerializeField] private float spawnRangeX;
    [SerializeField] private float spawnRangeY;
    [SerializeField] private Color gizmoColor = Color.red;
    [SerializeField] private bool showGizmos = true;
    
    [Space(10)]
    public List<GameObject> enemiesCollection = new List<GameObject>();
    
    [Space(10)]
    [SerializeField] private bool canSpawn = true;
    [SerializeField] private float spawningCooldown;
    [SerializeField] private int amountSpawnedAtATime;
    [SerializeField] private int maxAmountSpawnedAtATime;
    [SerializeField] private int amountCurrentlySpawned;
    [SerializeField] private int timeBetweenEachSpawn;
    
    [SerializeField] private float currentCooldownTimer;

    private void Update()
    {
        if (_timer == null)
        {
            TrySubscribe();
            return;
        }
        
        if (!canSpawn) return;
        
        if (amountCurrentlySpawned <= maxAmountSpawnedAtATime)
        {
            currentCooldownTimer += Time.deltaTime;

            if (currentCooldownTimer >= spawningCooldown)
            {
                StartCoroutine(SpawnEnemies(amountSpawnedAtATime));
                currentCooldownTimer = 0f;
            }
            else 
            {
                StopCoroutine(SpawnEnemies(amountSpawnedAtATime));
            }
        }
    }

    private void TrySubscribe()
    {
        if (LevelTimerManager.Instance == null)
            return;

        _timer = LevelTimerManager.Instance;
        _timer.StateChanged += HandleStateChanged;
        canSpawn = _timer.CurrentDirection == TimerDirection.CountingUp; // sync to current state
    }
    
    private void HandleStateChanged(TimerDirection direction)
    {
        canSpawn = direction == TimerDirection.CountingUp;
    }

    private IEnumerator SpawnEnemies(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (amountCurrentlySpawned >= maxAmountSpawnedAtATime)
            {
                StopCoroutine(SpawnEnemies(amount));
                yield break;
            }
            
            int randomIndex = Random.Range(0, enemiesCollection.Count);
            GameObject enemyToSpawn = enemiesCollection[randomIndex];
            
            float randomX = Random.Range(-spawnRangeX, spawnRangeX);
            float randomY = Random.Range(-spawnRangeY, spawnRangeY);
            Vector3 randomSpawn = transform.position + new Vector3(randomX, randomY, 0);
            
            Instantiate(enemyToSpawn, randomSpawn, Quaternion.identity);
            amountCurrentlySpawned++;

            yield return new WaitForSeconds(timeBetweenEachSpawn);
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        Gizmos.color = gizmoColor;
        
        Vector3 center = transform.position;
        Vector3 size = new Vector3(spawnRangeX * 2, spawnRangeY * 2, 0.1f);
        Gizmos.DrawWireCube(center, size);
        
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.2f);
        Gizmos.DrawCube(center, size);
    }
    
    private void OnDestroy()
    {
        if (_timer != null)
            _timer.StateChanged -= HandleStateChanged;
    }
}
