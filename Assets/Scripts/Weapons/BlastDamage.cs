using System.Collections.Generic;
using UnityEngine;

// On a lobbed projectile: when it lands, damages every IDamageable within the radius once (enemies, bushes),
// with knockback away from the blast centre. The player is not IDamageable, so the thrower is never hurt.
[RequireComponent(typeof(FallingProjectile))]
public class BlastDamage : MonoBehaviour
{
    [SerializeField] private float blastRadius = 2.5f;

    private readonly List<Collider2D> overlappingColliders = new List<Collider2D>();
    private FallingProjectile fallingProjectile;
    private int damageAmount;
    private float knockbackForce;
    private bool isInitialized;

    private void Awake()
    {
        fallingProjectile = GetComponent<FallingProjectile>();
        fallingProjectile.Landed += Explode;
    }

    private void OnDestroy()
    {
        fallingProjectile.Landed -= Explode;
    }

    public void Initialize(WeaponInfo weaponInfo)
    {
        damageAmount = weaponInfo.weaponDamage;
        knockbackForce = weaponInfo.knockbackForce;
        isInitialized = true;
    }

    private void Explode(Vector2 blastCentre)
    {
        if (!isInitialized) { return; }

        Physics2D.OverlapCircle(blastCentre, blastRadius, ContactFilter2D.noFilter, overlappingColliders);

        // An enemy with several colliders is still hit only once.
        HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();
        DamageInfo damage = new DamageInfo(damageAmount, knockbackForce, blastCentre, DamageFlags.None);

        foreach (Collider2D overlappingCollider in overlappingColliders)
        {
            if (overlappingCollider.TryGetComponent(out IDamageable damageable) && damagedTargets.Add(damageable))
            {
                damageable.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}
