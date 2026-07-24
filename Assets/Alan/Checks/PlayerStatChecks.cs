using UnityEngine;

public class PlayerStatChecks : MonoBehaviour
{
    public static float DamageTaken;
    
    public static void TakeDamage(float damage) 
    {
        DamageTaken += damage;
    }
}
