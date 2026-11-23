using System;
using System.Collections;
using UnityEngine;

// Runs the burst/projectile timing of an AttackPattern. Each burst rolls its projectile count; the attacker
// decides how to aim (beginBurst: burst index, projectile count) and what to spawn (fireProjectile: index).
// Stops when the player is dead.
public static class AttackPatternRunner
{
    public static IEnumerator Run(AttackPattern pattern, Action<int, int> beginBurst, Action<int> fireProjectile)
    {
        for (int burstIndex = 0; burstIndex < pattern.burstCount; burstIndex++)
        {
            if (PlayerHealth.Instance.IsDead) { yield break; }

            int projectileCount = pattern.RollProjectileCount();
            beginBurst(burstIndex, projectileCount);

            for (int projectileIndex = 0; projectileIndex < projectileCount; projectileIndex++)
            {
                fireProjectile(projectileIndex);

                if (pattern.timeBetweenProjectiles > 0f)
                {
                    yield return new WaitForSeconds(pattern.timeBetweenProjectiles);
                }
            }

            if (pattern.timeBetweenBursts > 0f)
            {
                yield return new WaitForSeconds(pattern.timeBetweenBursts);
            }
        }

        yield return new WaitForSeconds(pattern.restTime);
    }
}
