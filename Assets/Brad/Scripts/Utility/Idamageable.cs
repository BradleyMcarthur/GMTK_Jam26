/// <summary>
/// Anything that can take damage implements this — enemies, breakable
/// objects, destructible cover, etc. Projectiles (and anything else that
/// deals damage) only need to know about this, so new damageable things need zero changes to Projectile.
/// </summary>
public interface IDamageable
{
    void TakeDamage(float amount);
}
