using UnityEngine;

public class ActiveInventory : MonoBehaviour
{
    private int activeItemIndex = 0;

    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void Start()
    {
        playerControls.Inventory.Keyboard.performed += ctx => ToggleActiveSlot((int)ctx.ReadValue<float>());

        ToggleActiveHighlight(0);
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void ToggleActiveSlot(int index)
    {
        ToggleActiveHighlight(index -1);
    }
    private void ToggleActiveHighlight(int index)
    {
        activeItemIndex = index;

        // guard against invalid index
        if (activeItemIndex < 0 || activeItemIndex >= transform.childCount)
            return;

        foreach(Transform inventorySlot in transform)
        {
            inventorySlot.GetChild(0).gameObject.SetActive(false);
        }

        this.transform.GetChild(activeItemIndex).GetChild(0).gameObject.SetActive(true);

        ChangeActiveWeapon();
    }

    private void ChangeActiveWeapon()
    {
        if (ActiveWeapon.Instance.CurrentActiveWeapon != null)
        {
            Destroy(ActiveWeapon.Instance.CurrentActiveWeapon.gameObject); 
        }
        if (!transform.GetChild(activeItemIndex).GetComponentInChildren<InventorySlot>())
        {
            ActiveWeapon.Instance.WeaponNull();
            return;
        }

        GameObject weaponToSpawn = transform.GetChild(activeItemIndex).GetComponentInChildren<InventorySlot>().GetWeaponInfo().weaponPrefab;

        GameObject newWeapon = Instantiate(weaponToSpawn, ActiveWeapon.Instance.transform.position, Quaternion.identity);

        newWeapon.transform.parent = ActiveWeapon.Instance.transform;

        // Ensure the spawned prefab has a component that implements IWeapon
        var weaponInterface = newWeapon.GetComponent<IWeapon>();
        if (weaponInterface == null)
        {
            Debug.LogError($"Spawned weapon prefab '{weaponToSpawn.name}' has no component implementing IWeapon.");
            Destroy(newWeapon);
            ActiveWeapon.Instance.WeaponNull();
            return;
        }

        // store the actual component (cast to MonoBehaviour so existing API works)
        ActiveWeapon.Instance.NewWeapon(weaponInterface as MonoBehaviour);
    }
}
