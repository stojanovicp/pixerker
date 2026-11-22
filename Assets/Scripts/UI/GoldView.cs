using UnityEngine;
using TMPro;

public class GoldView : MonoBehaviour
{
    private const string GOLD_FORMAT = "D3";

    [SerializeField] private TMP_Text goldText;

    private void OnEnable()
    {
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
