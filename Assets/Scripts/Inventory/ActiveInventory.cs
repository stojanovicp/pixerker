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

        foreach(Transform inventorySlot in transform)
        {
            inventorySlot.GetChild(0).gameObject.SetActive(false);
        }

        this.transform.GetChild(activeItemIndex).GetChild(0).gameObject.SetActive(true);
    }
}
