using UnityEngine;

// Burst and timing data shared by enemy attackers (Shooter fires aimed bullets, Grape lobs projectiles).
[CreateAssetMenu(menuName = "Attack Pattern")]
public class AttackPattern : ScriptableObject
{
    [Header("Bursts (all attackers)")]
    [Min(1)] public int burstCount = 1;
    [Min(1)] public int projectilesPerBurst = 1;
    [Tooltip("Delay after each projectile. 0 fires the whole burst at once.")]
    [Min(0f)] public float timeBetweenProjectiles = 0f;
    [Tooltip("Extra delay after each burst.")]
    [Min(0f)] public float timeBetweenBursts = 0.5f;
    [Tooltip("Pause after the last burst before the attacker accepts a new attack.")]
    [Min(0f)] public float restTime = 0.1f;

    [Header("Aimed projectiles (Shooter)")]
    [Tooltip("Width of the cone centred on the player, in degrees. 0 fires every projectile straight at the player.")]
    [Range(0f, 359f)] public float angleSpread = 0f;
    [Tooltip("Odd bursts sweep back across the previous cone instead of re-aiming.")]
    public bool oscillate;
    [Min(0.1f)] public float spawnDistance = 0.3f;
    [Min(0.1f)] public float projectileSpeed = 7f;
    [Tooltip("Damage each bullet deals to the player.")]
    [Min(0)] public int projectileDamage = 8;

    [Header("Lobbed projectiles (Grape)")]
    [Tooltip("The first projectile of a burst lands on the target; the others land within this radius of it.")]
    [Min(0f)] public float targetScatterRadius = 0f;
}
