using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Shows the victory panel when the final boss dies: freezes the game, shows the gold as the score and offers
// Play Again / Main Menu / Quit. Lives on an always-active object in the persistent UI and toggles the panel.
public class VictoryScreenView : MonoBehaviour
{
    private const string SCORE_FORMAT = "D3";

    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    [Tooltip("Delay so the boss's death can be seen before the screen appears. Scaled time, so it waits while the pause menu is open.")]
    [SerializeField] private float showDelay = 1.5f;
    [SerializeField] private string firstSceneName = "Scene_1";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        if (victoryPanel == null || scoreText == null || playAgainButton == null || mainMenuButton == null || quitButton == null)
        {
            Debug.LogError($"{nameof(VictoryScreenView)} on '{name}': assign the panel, score text and the three buttons.", this);
            enabled = false;
            return;
        }

        victoryPanel.SetActive(false);
        playAgainButton.onClick.AddListener(() => GameSession.StartNewGame(firstSceneName));
        mainMenuButton.onClick.AddListener(() => GameSession.ReturnToMainMenu(mainMenuSceneName));
        GameSession.BindQuitButton(quitButton);
    }

    private void OnEnable()
    {
        Boss.FinalBossDefeated += OnFinalBossDefeated;
    }

    private void OnDisable()
    {
        Boss.FinalBossDefeated -= OnFinalBossDefeated;
    }

    private void OnFinalBossDefeated()
    {
        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        yield return new WaitForSeconds(showDelay);

        scoreText.text = $"Gold: {EconomyManager.Instance.CurrentGold.ToString(SCORE_FORMAT)}";
        victoryPanel.SetActive(true);
        GamePause.Pause();
    }
}
