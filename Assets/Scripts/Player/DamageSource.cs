using UnityEngine;

public class DamageSource : MonoBehaviour
{
    private int damageAmount;
    private float knockbackForce;
    private bool isInitialized;

    public void Initialize(WeaponInfo weaponInfo)
    {
        damageAmount = weaponInfo.weaponDamage;
        knockbackForce = weaponInfo.knockbackForce;
        isInitialized = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isInitialized) { return; }

        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(new DamageInfo(damageAmount, knockbackForce, transform.position, DamageFlags.None));
        }
    }
}
