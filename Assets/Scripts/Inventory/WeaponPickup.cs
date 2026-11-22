using UnityEngine;

// A weapon lying in the world. Fully described by its WeaponInfo and position, so world state can save and respawn it.
public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public WeaponInfo WeaponInfo => weaponInfo;

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

    // A dropped weapon lands on the player, so it only arms after the player steps off it.
    public void InitializeAsDropped(WeaponInfo droppedWeaponInfo)
    {
        weaponInfo = droppedWeaponInfo;
        isArmed = false;
        RefreshSprite();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isArmed || isCollected || !other.GetComponent<PlayerController>()) { return; }
        if (!ActiveInventory.Instance.TryAddWeapon(weaponInfo)) { return; }

        isCollected = true;
        Destroy(gameObject);
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
