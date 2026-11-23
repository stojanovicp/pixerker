using System;
using System.Collections;
using UnityEngine;

// Runs the burst/projectile timing of an AttackPattern. The attacker decides how to aim (beginBurst)
// and what to spawn (fireProjectile). Stops when the player is dead.
public static class AttackPatternRunner
{
    public static IEnumerator Run(AttackPattern pattern, Action<int> beginBurst, Action<int> fireProjectile)
    {
        for (int burstIndex = 0; burstIndex < pattern.burstCount; burstIndex++)
        {
            if (PlayerHealth.Instance.IsDead) { yield break; }

            beginBurst(burstIndex);

            for (int projectileIndex = 0; projectileIndex < pattern.projectilesPerBurst; projectileIndex++)
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
