using UnityEngine;
using UnityEngine.UI;

public class HealthBarView : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    private void OnEnable()
    {
        if (healthSlider == null)
        {
            Debug.LogError($"{nameof(HealthBarView)} on '{name}': Health Slider is not assigned.", this);
            return;
        }

        PlayerHealth.HealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        PlayerHealth.HealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
}
