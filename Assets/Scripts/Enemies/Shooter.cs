using System.Collections;
using UnityEngine;

// Fires bullets in a cone centred on the player, following an AttackPattern. Attack uses the default pattern;
// a Boss passes its own patterns through AttackWith.
public class Shooter : MonoBehaviour, IEnemy, IPatternAttacker
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private AttackPattern attackPattern;

    private AttackPattern currentPattern;
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
        AttackWith(attackPattern);
    }

    public void AttackWith(AttackPattern pattern)
    {
        if (!enabled || isShooting || pattern == null) { return; }

        StartCoroutine(ShootRoutine(pattern));
    }

    private IEnumerator ShootRoutine(AttackPattern pattern)
    {
        isShooting = true;
        currentPattern = pattern;
        yield return AttackPatternRunner.Run(pattern, BeginBurst, FireBullet);
        isShooting = false;
    }

    // Even bursts (or every burst without oscillate) re-aim the cone at the player and sweep it from one
    // edge to the other; odd bursts with oscillate sweep the previous cone back the other way.
    private void BeginBurst(int burstIndex, int projectileCount)
    {
        burstProjectileCount = projectileCount;

        bool sweepsBack = currentPattern.oscillate && burstIndex % 2 == 1;

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
        float halfSpread = currentPattern.angleSpread / 2f;

        sweepFromAngle = angleToPlayer - halfSpread;
        sweepToAngle = angleToPlayer + halfSpread;
    }

    // A single projectile goes through the middle of the cone, i.e. straight at the player.
    private void FireBullet(int projectileIndex)
    {
        float sweepProgress = burstProjectileCount > 1 ? (float)projectileIndex / (burstProjectileCount - 1) : 0.5f;
        float angle = Mathf.Lerp(sweepFromAngle, sweepToAngle, sweepProgress) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Vector2 spawnPosition = (Vector2)transform.position + direction * currentPattern.spawnDistance;

        GameObject newBullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        newBullet.transform.right = direction;

        if (newBullet.TryGetComponent(out Projectile projectile))
        {
            projectile.UpdateMoveSpeed(currentPattern.projectileSpeed);
            projectile.SetEnemyProjectileDamage(currentPattern.projectileDamage);
        }
    }
}
