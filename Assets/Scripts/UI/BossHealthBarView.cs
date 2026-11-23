using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Shows the engaged boss's name and health. Lives on an always-active object and toggles the bar panel.
public class BossHealthBarView : MonoBehaviour
{
    [SerializeField] private GameObject barPanel;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text nameText;

    private Boss trackedBoss;

    private void Awake()
    {
        if (barPanel == null || healthSlider == null || nameText == null)
        {
            Debug.LogError($"{nameof(BossHealthBarView)} on '{name}': assign Bar Panel, Health Slider and Name Text.", this);
            enabled = false;
            return;
        }

        barPanel.SetActive(false);
    }

    private void OnEnable()
    {
        Boss.Engaged += Show;
        Boss.Disengaged += Hide;
    }

    private void OnDisable()
    {
        Boss.Engaged -= Show;
        Boss.Disengaged -= Hide;
        StopTracking();
    }

    private void Show(Boss boss)
    {
        StopTracking();

        trackedBoss = boss;
        trackedBoss.Health.HealthChanged += UpdateBar;

        nameText.text = boss.DisplayName;
        UpdateBar(boss.Health.CurrentHealth, boss.Health.MaxHealth);
        barPanel.SetActive(true);
    }

    private void Hide(Boss boss)
    {
        if (boss != trackedBoss) { return; }

        StopTracking();
        barPanel.SetActive(false);
    }

    private void StopTracking()
    {
        if (trackedBoss != null)
        {
            trackedBoss.Health.HealthChanged -= UpdateBar;
        }

        trackedBoss = null;
    }

    private void UpdateBar(int currentHealth, int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
}
