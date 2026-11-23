using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    // Share of a boss's max HP taken by an Execute hit (normal enemies lose all remaining HP).
    private const float BOSS_EXECUTE_FRACTION = 0.05f;

    public event Action<int, int> HealthChanged;
    public event Action Died;

    [SerializeField] private EnemyStats stats;
    [SerializeField] private GameObject deathVFXPrefab;

    public int CurrentHealth => Mathf.Max(currentHealth, 0);
    public int MaxHealth => stats.maxHealth;

    private int currentHealth;
    private bool isDead;
    private Knockback knockback;
    private Flash flash;

    private void Awake() {
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
        currentHealth = stats.maxHealth;
    }

    public int TakeDamage(DamageInfo damage) {
        int remainingHealth = CurrentHealth;
        bool isExecute = (damage.Flags & DamageFlags.Execute) != 0;
        int damageAmount = isExecute ? GetExecuteDamage(remainingHealth) : damage.Amount;

        currentHealth -= damageAmount;
        knockback.GetKnockedBack(damage.SourcePosition, damage.KnockbackForce * stats.knockbackMultiplier);
        StartCoroutine(flash.FlashRoutine());
        StartCoroutine(CheckDetectDeathRoutine());

        HealthChanged?.Invoke(CurrentHealth, MaxHealth);
        return Mathf.Min(damageAmount, remainingHealth);
    }

    // Immediate death with the normal death effects and drops (e.g. summons when their boss dies).
    public void Kill() {
        currentHealth = 0;
        DetectDeath();
    }

    private int GetExecuteDamage(int remainingHealth) {
        return stats.isBoss ? Mathf.CeilToInt(stats.maxHealth * BOSS_EXECUTE_FRACTION) : remainingHealth;
    }

    private IEnumerator CheckDetectDeathRoutine() {
        yield return new WaitForSeconds(flash.GetRestoreMatTime());
        DetectDeath();
    }

    public void DetectDeath() {
        if (isDead || currentHealth > 0) { return; }

        isDead = true;

        if (TryGetComponent(out PersistentObject persistentObject)) {
            persistentObject.MarkRemoved();
        }

        Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);

        if (TryGetComponent(out PickupSpawner pickupSpawner)) {
            pickupSpawner.DropItems();
        }

        Died?.Invoke();
        Destroy(gameObject);
    }
}
