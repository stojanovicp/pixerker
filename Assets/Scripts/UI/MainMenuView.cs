using UnityEngine;
using UnityEngine.UI;

// Main menu: Start begins a new game, Quit leaves (hidden on WebGL).
public class MainMenuView : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private string firstSceneName = "Scene_1";

    private void Awake()
    {
        if (startButton == null || quitButton == null)
        {
            Debug.LogError($"{nameof(MainMenuView)} on '{name}': assign Start Button and Quit Button.", this);
            enabled = false;
            return;
        }

        startButton.onClick.AddListener(() => GameSession.StartNewGame(firstSceneName));
        GameSession.BindQuitButton(quitButton);
    }
}
