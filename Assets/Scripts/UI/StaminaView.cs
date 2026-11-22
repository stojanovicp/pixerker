using UnityEngine;
using UnityEngine.UI;

public class StaminaView : MonoBehaviour
{
    [SerializeField] private Image[] staminaImages;
    [SerializeField] private Sprite fullStaminaSprite;
    [SerializeField] private Sprite emptyStaminaSprite;

    private void OnEnable()
    {
        if (!HasValidReferences())
        {
            Debug.LogError($"{nameof(StaminaView)} on '{name}': assign both sprites and at least one stamina image, with no empty entries.", this);
            return;
        }

        Stamina.StaminaChanged += UpdateStaminaImages;
    }

    private void OnDisable()
    {
        Stamina.StaminaChanged -= UpdateStaminaImages;
    }

    private bool HasValidReferences()
    {
        if (fullStaminaSprite == null || emptyStaminaSprite == null) { return false; }
        if (staminaImages == null || staminaImages.Length == 0) { return false; }

        foreach (Image staminaImage in staminaImages)
        {
            if (staminaImage == null) { return false; }
        }

        return true;
    }

    private void UpdateStaminaImages(int currentStamina, int maxStamina)
    {
        for (int i = 0; i < staminaImages.Length; i++)
        {
            Image staminaImage = staminaImages[i];
            staminaImage.gameObject.SetActive(i < maxStamina);
            staminaImage.sprite = i < currentStamina ? fullStaminaSprite : emptyStaminaSprite;
        }
    }
}
