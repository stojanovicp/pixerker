using UnityEngine;

public class Destructible : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject destroyVFX;

    private bool isDestroyed;

    // Bushes have no health: they break on any hit and report no damage dealt (so no life steal).
    public int TakeDamage(DamageInfo damage) {
        if (isDestroyed) { return 0; }

        isDestroyed = true;

        if (TryGetComponent(out PersistentObject persistentObject)) {
            persistentObject.MarkRemoved();
        }

        if (TryGetComponent(out PickupSpawner pickupSpawner)) {
            pickupSpawner.DropItems();
        }

        Instantiate(destroyVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
        return 0;
    }
}
