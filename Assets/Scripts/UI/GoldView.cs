using UnityEngine;
using TMPro;

public class GoldView : MonoBehaviour
{
    private const string GOLD_FORMAT = "D3";

    [SerializeField] private TMP_Text goldText;

    private void OnEnable()
    {
        if (goldText == null)
        {
            Debug.LogError($"{nameof(GoldView)} on '{name}': Gold Text is not assigned.", this);
            return;
        }

        EconomyManager.GoldChanged += UpdateGoldText;
    }

    private void OnDisable()
    {
        EconomyManager.GoldChanged -= UpdateGoldText;
    }

    private void UpdateGoldText(int gold)
    {
        goldText.text = gold.ToString(GOLD_FORMAT);
    }
}
