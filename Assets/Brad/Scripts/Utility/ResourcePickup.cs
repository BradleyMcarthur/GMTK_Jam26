using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ResourcePickup : MonoBehaviour
{
    [SerializeField] private float resourceValue = 10f;

    private void OnTriggerStay2D(Collider2D other)
    {
        // PlayerStats lives on the player, so its presence identifies the player without needing a separate tag check.
        if (!other.TryGetComponent<PlayerStats>(out _))
            return;
        
        if (PlayerProgression.Instance == null || !PlayerProgression.Instance.CanCollectResources)
            return;

        PlayerProgression.Instance.AddResource(resourceValue);
        Destroy(gameObject);
    }
}
