using UnityEngine;

public class DamageNumberSpawner : MonoBehaviour
{
    public static DamageNumberSpawner Instance { get; private set; }

    [SerializeField] private DamageNumber damageNumberPrefab;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 0.5f, 0f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnDamageNumber(Vector3 worldPosition, float damage, bool isCrit)
    {
        if (damageNumberPrefab == null)
            return;

        DamageNumber popup = Instantiate(damageNumberPrefab, worldPosition + spawnOffset, Quaternion.identity);
        popup.Initialize(damage, isCrit);
    }
}
