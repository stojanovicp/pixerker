using System.Collections;
using UnityEngine;

// Fires bullets in a cone centred on the player, following an AttackPattern.
public class Shooter : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private AttackPattern attackPattern;

    private bool isShooting;
    private float sweepFromAngle;
    private float sweepToAngle;
    private int burstProjectileCount;

    private void Awake()
    {
        if (bulletPrefab == null || attackPattern == null)
        {
            Debug.LogError($"{nameof(Shooter)} on '{name}': assign Bullet Prefab and Attack Pattern.", this);
            enabled = false;
        }
    }

    public void Attack()
    {
        if (!enabled || isShooting) { return; }

        StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        isShooting = true;
        yield return AttackPatternRunner.Run(attackPattern, BeginBurst, FireBullet);
        isShooting = false;
    }

    // Even bursts (or every burst without oscillate) re-aim the cone at the player and sweep it from one
    // edge to the other; odd bursts with oscillate sweep the previous cone back the other way.
    private void BeginBurst(int burstIndex, int projectileCount)
    {
        burstProjectileCount = projectileCount;

        bool sweepsBack = attackPattern.oscillate && burstIndex % 2 == 1;

        if (sweepsBack)
        {
            (sweepFromAngle, sweepToAngle) = (sweepToAngle, sweepFromAngle);
            return;
        }

        AimConeAtPlayer();
    }

    private void AimConeAtPlayer()
    {
        Vector2 directionToPlayer = PlayerController.Instance.transform.position - transform.position;
        float angleToPlayer = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        float halfSpread = attackPattern.angleSpread / 2f;

        sweepFromAngle = angleToPlayer - halfSpread;
        sweepToAngle = angleToPlayer + halfSpread;
    }

    // A single projectile goes through the middle of the cone, i.e. straight at the player.
    private void FireBullet(int projectileIndex)
    {
        float sweepProgress = burstProjectileCount > 1 ? (float)projectileIndex / (burstProjectileCount - 1) : 0.5f;
        float angle = Mathf.Lerp(sweepFromAngle, sweepToAngle, sweepProgress) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Vector2 spawnPosition = (Vector2)transform.position + direction * attackPattern.spawnDistance;

        GameObject newBullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        newBullet.transform.right = direction;

        if (newBullet.TryGetComponent(out Projectile projectile))
        {
            projectile.UpdateMoveSpeed(attackPattern.projectileSpeed);
            projectile.SetEnemyProjectileDamage(attackPattern.projectileDamage);
        }
    }
}
