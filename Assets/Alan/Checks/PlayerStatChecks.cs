using System;
using UnityEngine;

public class PlayerStatChecks : MonoBehaviour
{
    [SerializeField] private static float DamageTakenCodeOnly;
    public float damageTaken;
    
    public static void TakeDamage(float damage) 
    {
        DamageTakenCodeOnly += damage;
    }

    private void FixedUpdate()
    {
        damageTaken =  DamageTakenCodeOnly;
    }
}
