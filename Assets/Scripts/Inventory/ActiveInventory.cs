using UnityEngine;

public class ActiveInventory : Singleton<ActiveInventory>
{
    [SerializeField] private InventorySlot[] slots;
    [SerializeField] private WeaponPickup weaponPickupPrefab;
    [SerializeField] private Vector2 dropOffset = new Vector2(0f, -0.3f);

    private int activeSlotIndex = 0;
    private PlayerControls playerControls;

    protected override void Awake() {
        base.Awake();

        if (Instance != this) { return; }

        if (slots == null || slots.Length == 0 || weaponPickupPrefab == null)
        {
            Debug.LogError($"{nameof(ActiveInventory)} on '{name}': assign Slots and Weapon Pickup Prefab.", this);
        }

        playerControls = new PlayerControls();
    }

    private void Start()
    {
        playerControls.Inventory.Keyboard.performed += ctx => SelectSlot((int)ctx.ReadValue<float>() - 1);
        playerControls.Inventory.Drop.performed += _ => DropActiveWeapon();
    }

    private void OnEnable() {
        playerControls?.Enable();
    }

    private void OnDisable() {
        playerControls?.Disable();
    }

    public void EquipStartingWeapon()
    {
        SelectSlot(0);
    }

    public bool HasWeapon(WeaponInfo weaponInfo)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.GetWeaponInfo() == weaponInfo) { return true; }
        }

        return false;
    }

    // Puts the weapon in the first free slot. Returns false when dead, already owned or full.
    public bool TryAddWeapon(WeaponInfo weaponInfo)
    {
        if (PlayerHealth.Instance.IsDead || HasWeapon(weaponInfo)) { return false; }

        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsEmpty) { continue; }

            slots[i].SetWeapon(weaponInfo);

            if (i == activeSlotIndex)
            {
                EquipActiveSlotWeapon();
            }

            return true;
        }

        return false;
    }

    private void DropActiveWeapon()
    {
        if (PlayerHealth.Instance.IsDead) { return; }

        InventorySlot activeSlot = slots[activeSlotIndex];
        WeaponInfo droppedWeaponInfo = activeSlot.GetWeaponInfo();

        if (droppedWeaponInfo == null || !droppedWeaponInfo.canBeDropped) { return; }

        activeSlot.Clear();
        EquipActiveSlotWeapon();

        Vector2 dropPosition = (Vector2)PlayerController.Instance.transform.position + dropOffset;
        WeaponPickup droppedPickup = Instantiate(weaponPickupPrefab, dropPosition, Quaternion.identity);
        droppedPickup.InitializeAsDropped(droppedWeaponInfo);
    }

    private void SelectSlot(int slotIndex) {
        if (slotIndex < 0 || slotIndex >= slots.Length) { return; }

        activeSlotIndex = slotIndex;

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetHighlighted(i == activeSlotIndex);
        }

        EquipActiveSlotWeapon();
    }

    private void EquipActiveSlotWeapon() {
        if (PlayerHealth.Instance.IsDead) { return; }

        if (ActiveWeapon.Instance.CurrentActiveWeapon != null)
        {
            Destroy(ActiveWeapon.Instance.CurrentActiveWeapon.gameObject);
        }

        WeaponInfo weaponInfo = slots[activeSlotIndex].GetWeaponInfo();

        if (weaponInfo == null)
        {
            ActiveWeapon.Instance.WeaponNull();
            return;
        }

        GameObject newWeapon = Instantiate(weaponInfo.weaponPrefab, ActiveWeapon.Instance.transform.position, Quaternion.identity);
        ActiveWeapon.Instance.transform.rotation = Quaternion.identity;
        newWeapon.transform.parent = ActiveWeapon.Instance.transform;

        ActiveWeapon.Instance.NewWeapon(newWeapon.GetComponent<MonoBehaviour>());
    }
}
