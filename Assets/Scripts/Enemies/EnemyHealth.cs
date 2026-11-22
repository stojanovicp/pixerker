using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int startingHealth = 3;
    [SerializeField] private GameObject deathVFXPrefab;

    private int currentHealth;
    private bool isDead;
    private Knockback knockback;
    private Flash flash;

    private void Awake() {
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
    }

    private void Start() {
        currentHealth = startingHealth;
    }

    public void TakeDamage(DamageInfo damage) {
        currentHealth -= damage.Amount;
        knockback.GetKnockedBack(damage.SourcePosition, damage.KnockbackForce);
        StartCoroutine(flash.FlashRoutine());
        StartCoroutine(CheckDetectDeathRoutine());
    }

    private IEnumerator CheckDetectDeathRoutine() {
        yield return new WaitForSeconds(flash.GetRestoreMatTime());
        DetectDeath();
    }

    public void DetectDeath() {
        if (isDead || currentHealth > 0) { return; }

        isDead = true;
        Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);

        if (TryGetComponent(out PickupSpawner pickupSpawner)) {
            pickupSpawner.DropItems();
        }

        Destroy(gameObject);
    }
}
