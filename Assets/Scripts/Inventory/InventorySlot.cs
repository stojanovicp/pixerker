using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private GameObject activeHighlight;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownFillImage;

    public bool IsEmpty => weaponInfo == null;

    private void Awake()
    {
        if (activeHighlight == null || iconImage == null || cooldownFillImage == null)
        {
            Debug.LogError($"{nameof(InventorySlot)} on '{name}': assign Active Highlight, Icon Image and Cooldown Fill Image.", this);
            enabled = false;
            return;
        }

        RefreshIcon();
    }

    private void Update()
    {
        UpdateCooldownFill();
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }

    public void SetWeapon(WeaponInfo newWeaponInfo)
    {
        weaponInfo = newWeaponInfo;
        RefreshIcon();
    }

    public void Clear()
    {
        SetWeapon(null);
    }

    public void SetHighlighted(bool isHighlighted)
    {
        activeHighlight.SetActive(isHighlighted);
    }

    private void RefreshIcon()
    {
        iconImage.sprite = IsEmpty ? null : weaponInfo.icon;
        iconImage.enabled = !IsEmpty;
    }

    private void UpdateCooldownFill()
    {
        bool showsCooldown = !IsEmpty && ActiveWeapon.Instance != null;
        cooldownFillImage.fillAmount = showsCooldown ? ActiveWeapon.Instance.GetCooldownRemainingFraction(weaponInfo) : 0f;
    }
}
