using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    private LevelTimerManager _timer;
    private GameManager _gameManager;
    
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

    [Space(15)]
    public bool onCountUp = false;
    [SerializeField] private float specialSpawningCooldown;
    [SerializeField] private int specialAmountSpawnedAtATime;
    [SerializeField] private int specialMaxAmountSpawnedAtATime;
    [SerializeField] private int specialTimeBetweenEachSpawn;
    
    [Space(10)]
    [SerializeField] private float originalValueOfSpawningCooldown;
    [SerializeField] private int originalValueOfAmountSpawnedAtATime;
    [SerializeField] private int originalValueOfMaxAmountSpawnedAtATime;
    [SerializeField] private int originalValueOfTimeBetweenEachSpawn;
    
    private Coroutine _spawnEnemiesCoroutine;

    private void Awake()
    {
        originalValueOfSpawningCooldown = spawningCooldown;
        originalValueOfAmountSpawnedAtATime = amountSpawnedAtATime;
        originalValueOfMaxAmountSpawnedAtATime = maxAmountSpawnedAtATime;
        originalValueOfTimeBetweenEachSpawn = timeBetweenEachSpawn;
    }

    private void Update()
    {
        if (_timer == null)
        {
            TrySubscribeToTimer();
            return;
        }

        if (_gameManager == null)
            TrySubscribeToGameManager();

        if (onCountUp)
        {
            OnCountUpEvent();
        }
        else
        {
            BackToCountDown();
        }

        if (!canSpawn)
        {
            // Spawning is off (counting down) so halts any burst in progress
            // rather than letting it keep spawning enemies from before.
            if (_spawnEnemiesCoroutine != null)
            {
                StopCoroutine(_spawnEnemiesCoroutine);
                _spawnEnemiesCoroutine = null;
            }
            return;
        }

        if (amountCurrentlySpawned >= maxAmountSpawnedAtATime)
            return;

        currentCooldownTimer += Time.deltaTime;

        // Only start a new burst once the previous one has actually finished —
        // previously this also tried to StopCoroutine every frame the cooldown
        // hadn't been reached yet, which killed the coroutine almost immediately
        // after starting it, nearly every time.
        if (currentCooldownTimer >= spawningCooldown && _spawnEnemiesCoroutine == null)
        {
            _spawnEnemiesCoroutine = StartCoroutine(SpawnEnemies(amountSpawnedAtATime));
            currentCooldownTimer = 0f;
        }
    }

    private void TrySubscribeToTimer()
    {
        if (LevelTimerManager.Instance == null)
            return;

        _timer = LevelTimerManager.Instance;
        _timer.StateChanged += HandleStateChanged;
        canSpawn = _timer.CurrentDirection == TimerDirection.CountingUp;
    }

    private void TrySubscribeToGameManager()
    {
        if (GameManager.Instance == null)
            return;

        _gameManager = GameManager.Instance;
        _gameManager.EnemyFrenzyTriggered += HandleFrenzyTriggered;
    }
    
    private void HandleStateChanged(TimerDirection direction)
    {
        canSpawn = direction == TimerDirection.CountingUp;

        // Frenzy mode only makes sense while actively counting up, ends it the moment the player swaps back to counting down.
        if (direction == TimerDirection.CountingDown)
            onCountUp = false;
    }

    private void HandleFrenzyTriggered()
    {
        onCountUp = true;
    }

    private IEnumerator SpawnEnemies(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (amountCurrentlySpawned >= maxAmountSpawnedAtATime)
                break;
            
            int randomIndex = Random.Range(0, enemiesCollection.Count);
            GameObject enemyToSpawn = enemiesCollection[randomIndex];
            
            float randomX = Random.Range(-spawnRangeX, spawnRangeX);
            float randomY = Random.Range(-spawnRangeY, spawnRangeY);
            Vector3 randomSpawn = transform.position + new Vector3(randomX, randomY, 0);
            
            Instantiate(enemyToSpawn, randomSpawn, Quaternion.identity);
            amountCurrentlySpawned++;

            yield return new WaitForSeconds(timeBetweenEachSpawn);
        }

        // Clear the reference once the burst finishes naturally, so Update() knows it's safe to start the next one.
        _spawnEnemiesCoroutine = null;
    }

    private void OnCountUpEvent()
    {
        spawningCooldown = specialSpawningCooldown;
        amountSpawnedAtATime =  specialAmountSpawnedAtATime;
        maxAmountSpawnedAtATime =  specialMaxAmountSpawnedAtATime;
        timeBetweenEachSpawn  = specialTimeBetweenEachSpawn;
    }

    private void BackToCountDown()
    {
        spawningCooldown = originalValueOfSpawningCooldown;
        amountSpawnedAtATime = originalValueOfAmountSpawnedAtATime;
        maxAmountSpawnedAtATime = originalValueOfMaxAmountSpawnedAtATime;
        timeBetweenEachSpawn = originalValueOfTimeBetweenEachSpawn;
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

        if (_gameManager != null)
            _gameManager.EnemyFrenzyTriggered -= HandleFrenzyTriggered;
    }
}