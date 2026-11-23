using UnityEngine;

// A weapon lying in the world. Either placed in a scene (persisted through PersistentObject)
// or dropped at runtime (persisted through its DroppedWeaponRecord in WorldState).
public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [Tooltip("Disabled on the prefab; only weapons dropped on death turn it on.")]
    [SerializeField] private PickupMagnet magnet;

    [Header("Pop-out arc (dropped weapons spawned away from their landing position)")]
    [SerializeField] private AnimationCurve popCurve;
    [SerializeField] private float popHeight = 1.3f;
    [SerializeField] private float popDuration = 0.4f;

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

        if (droppedRecord != null && (Vector2)transform.position != droppedRecord.Position)
        {
            StartCoroutine(ArcMovement.Move(transform, transform.position, droppedRecord.Position, popCurve, popHeight, popDuration));
        }
    }

    public void InitializeAsDropped(DroppedWeaponRecord record, bool armImmediately)
    {
        droppedRecord = record;
        weaponInfo = record.WeaponInfo;
        isArmed = armImmediately;
        magnet.enabled = record.AttractsToPlayer;
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
