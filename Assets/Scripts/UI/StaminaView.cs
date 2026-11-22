using UnityEngine;
using UnityEngine.UI;

public class StaminaView : MonoBehaviour
{
    [SerializeField] private Image[] staminaImages;
    [SerializeField] private Sprite fullStaminaSprite;
    [SerializeField] private Sprite emptyStaminaSprite;

    private void OnEnable()
    {
        Stamina.StaminaChanged += UpdateStaminaImages;
    }

    private void OnDisable()
    {
        Stamina.StaminaChanged -= UpdateStaminaImages;
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
