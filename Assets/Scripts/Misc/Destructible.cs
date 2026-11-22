using UnityEngine;

public class Destructible : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject destroyVFX;

    private bool isDestroyed;

    public void TakeDamage(DamageInfo damage) {
        if (isDestroyed) { return; }

        isDestroyed = true;

        if (TryGetComponent(out PersistentObject persistentObject)) {
            persistentObject.MarkRemoved();
        }

        if (TryGetComponent(out PickupSpawner pickupSpawner)) {
            pickupSpawner.DropItems();
        }

        Instantiate(destroyVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
