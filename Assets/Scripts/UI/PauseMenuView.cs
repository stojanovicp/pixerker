using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Esc or P opens and closes the pause menu: Resume, Main Menu, Quit. It cannot open while the player is dead or while
// something else (the victory screen) has paused the game. Lives on an always-active object in the persistent UI.
public class PauseMenuView : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private PlayerControls playerControls;
    private bool isOpen;

    private void Awake()
    {
        if (pausePanel == null || resumeButton == null || mainMenuButton == null || quitButton == null)
        {
            Debug.LogError($"{nameof(PauseMenuView)} on '{name}': assign the panel and the three buttons.", this);
            enabled = false;
            return;
        }

        playerControls = new PlayerControls();
        pausePanel.SetActive(false);
        resumeButton.onClick.AddListener(Close);
        mainMenuButton.onClick.AddListener(() => GameSession.ReturnToMainMenu(mainMenuSceneName));
        GameSession.BindQuitButton(quitButton);
    }

    private void OnEnable()
    {
        if (playerControls == null) { return; }

        playerControls.Menu.Pause.performed += OnPausePressed;
        playerControls.Menu.Enable();
    }

    private void OnDisable()
    {
        if (playerControls == null) { return; }

        playerControls.Menu.Pause.performed -= OnPausePressed;
        playerControls.Menu.Disable();
    }

    private void OnDestroy()
    {
        playerControls?.Dispose();
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (isOpen)
        {
            Close();
            return;
        }

        if (GamePause.IsPaused || PlayerHealth.Instance.IsDead) { return; }

        Open();
    }

    private void Open()
    {
        isOpen = true;
        pausePanel.SetActive(true);
        GamePause.Pause();
    }

    private void Close()
    {
        if (!isOpen) { return; }

        isOpen = false;
        pausePanel.SetActive(false);
        GamePause.Resume();
    }
}
