using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class HazardManager : MonoBehaviour
{
    private static HazardManager instance;
    
    [Header("References")]
    public Transform playerTransform;
    public GameObject alarmTimeBombPrefab;
    
    [Space(10)] [Header("Visualisation Controls")]
    [SerializeField] private float radius;
    [SerializeField] private Color gizmoColor = Color.red;
    [SerializeField] private bool showGizmos = true;
    
    [Space(10)] [Header("Hazard Spawn Controls")]
    [SerializeField] private float theCooldownForHazardSpawn;
    [SerializeField] private float timeBeforeNextHazardWaveSpawn;
    [SerializeField] private int amountOfHazardsThatCanSpawnAtOnce;
    [SerializeField] private float timeBetweenSpawns;
    
    [Space(10)] [Header("Hazard Debug Checks")]
    [SerializeField] private bool isCountingUp;
    
    private Coroutine spawnCoroutine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        if  (!playerTransform)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        timeBeforeNextHazardWaveSpawn = theCooldownForHazardSpawn;
    }

    private void Update()
    {
        if (!isCountingUp) return;
        
        if (timeBeforeNextHazardWaveSpawn > 0)
        {
            timeBeforeNextHazardWaveSpawn -= Time.deltaTime;
            return;
        }
        
        timeBeforeNextHazardWaveSpawn = theCooldownForHazardSpawn;
        
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnHazardsWithDelay());
        }
    }
    
    private IEnumerator SpawnHazardsWithDelay()
    {
        for (int i = 0; i < amountOfHazardsThatCanSpawnAtOnce; i++)
        {
            SpawnAlarmBombHazard();
            
            if (i < amountOfHazardsThatCanSpawnAtOnce - 1)
            {
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }
        
        spawnCoroutine = null;
    }

    private void SpawnAlarmBombHazard()
    {
        Vector3 randomPosition = GetRandomPositionInRadius();
        
        Instantiate(alarmTimeBombPrefab, randomPosition, Quaternion.identity);
    }
    
    private Vector3 GetRandomPositionInRadius()
    {
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * radius;
        Vector3 randomPosition = playerTransform.position + new Vector3(randomCircle.x, randomCircle.y, 0);
        
        return randomPosition;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
    
        Gizmos.color = gizmoColor;
    
        #if UNITY_EDITOR
        Vector3 center = playerTransform.transform.position;
        Handles.DrawWireDisc(center, Vector3.forward, radius);
    
        Handles.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.2f);
        Handles.DrawSolidDisc(center, Vector3.forward, radius);
        #endif  
    }
}
