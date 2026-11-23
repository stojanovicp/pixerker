using UnityEngine;

public class DamageSource : MonoBehaviour
{
    private int damageAmount;
    private float knockbackForce;
    private DamageFlags damageFlags;
    private float lifeStealFraction;
    private bool isInitialized;

    public void Initialize(WeaponInfo weaponInfo)
    {
        damageAmount = weaponInfo.weaponDamage;
        knockbackForce = weaponInfo.knockbackForce;
        damageFlags = weaponInfo.damageFlags;
        lifeStealFraction = weaponInfo.lifeStealFraction;
        isInitialized = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isInitialized) { return; }

        if (other.TryGetComponent(out IDamageable damageable))
        {
            int damageDealt = damageable.TakeDamage(new DamageInfo(damageAmount, knockbackForce, transform.position, damageFlags));
            StealLife(damageDealt);
        }
    }

    private void StealLife(int damageDealt)
    {
        int healAmount = Mathf.RoundToInt(damageDealt * lifeStealFraction);

        if (healAmount > 0)
        {
            PlayerHealth.Instance.HealPlayer(healAmount);
        }
    }
}
