using UnityEngine;

// A weapon lying in the world. Either placed in a scene (persisted through PersistentObject)
// or dropped at runtime (persisted through its DroppedWeaponRecord in WorldState).
public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public WeaponInfo WeaponInfo => weaponInfo;

    private DroppedWeaponRecord droppedRecord;
    private bool isArmed = true;
    private bool isCollected;

    private void OnValidate()
    {
        RefreshSprite();
    }

    private void Start()
    {
        RefreshSprite();
    }

    public void InitializeAsDropped(DroppedWeaponRecord record, bool armImmediately)
    {
        droppedRecord = record;
        weaponInfo = record.WeaponInfo;
        isArmed = armImmediately;
        RefreshSprite();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isArmed || isCollected || !other.GetComponent<PlayerController>()) { return; }
        if (!ActiveInventory.Instance.TryAddWeapon(weaponInfo)) { return; }

        isCollected = true;
        RecordCollected();
        Destroy(gameObject);
    }

    private void RecordCollected()
    {
        if (droppedRecord != null)
        {
            WorldState.RemoveDroppedWeapon(droppedRecord);
        }
        else if (TryGetComponent(out PersistentObject persistentObject))
        {
            persistentObject.MarkRemoved();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>())
        {
            isArmed = true;
        }
    }

    private void RefreshSprite()
    {
        if (spriteRenderer == null || weaponInfo == null) { return; }

        spriteRenderer.sprite = weaponInfo.icon;
    }
}
