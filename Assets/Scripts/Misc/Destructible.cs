using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] private GameObject destroyVFX;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.GetComponent<DamageSource>()) {
            PickupSpawner pickupSpawner = GetComponent<PickupSpawner>();
            if (pickupSpawner != null) pickupSpawner.DropItems();
            
            Instantiate(destroyVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
