using System.Collections;
using UnityEngine;

// Fires bullets in a cone centred on the player, following an AttackPattern.
public class Shooter : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private AttackPattern attackPattern;

    private bool isShooting;
    private float burstStartAngle;
    private float angleStep;

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

    private void BeginBurst(int burstIndex)
    {
        bool sweepsBack = attackPattern.oscillate && burstIndex % 2 == 1;

        if (sweepsBack)
        {
            burstStartAngle += angleStep * (attackPattern.projectilesPerBurst - 1);
            angleStep = -angleStep;
            return;
        }

        AimConeAtPlayer();
    }

    private void AimConeAtPlayer()
    {
        Vector2 directionToPlayer = PlayerController.Instance.transform.position - transform.position;
        float angleToPlayer = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        if (attackPattern.angleSpread > 0f && attackPattern.projectilesPerBurst > 1)
        {
            angleStep = attackPattern.angleSpread / (attackPattern.projectilesPerBurst - 1);
            burstStartAngle = angleToPlayer - attackPattern.angleSpread / 2f;
        }
        else
        {
            angleStep = 0f;
            burstStartAngle = angleToPlayer;
        }
    }

    private void FireBullet(int projectileIndex)
    {
        float angle = (burstStartAngle + angleStep * projectileIndex) * Mathf.Deg2Rad;
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
