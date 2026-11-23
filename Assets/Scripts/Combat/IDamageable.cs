public interface IDamageable
{
    // Returns the damage actually dealt (never more than the target's remaining health).
    int TakeDamage(DamageInfo damage);
}
