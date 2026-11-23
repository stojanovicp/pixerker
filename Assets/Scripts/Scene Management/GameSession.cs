using UnityEngine;
using UnityEngine.SceneManagement;

// Starts, restarts and leaves a play session. A new session clears the in-memory world state and destroys the
// persistent roots (Player, Managers, UI_Canvas); the loaded scene's own copies then become the fresh instances.
// Destroy finishes at the end of this frame and the scene loads in the next one, so no duplicates survive.
public static class GameSession
{
    public static void StartNewGame(string firstSceneName)
    {
        ClearSession();
        SceneManager.LoadScene(firstSceneName);
    }

    public static void ReturnToMainMenu(string mainMenuSceneName)
    {
        ClearSession();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private static void ClearSession()
    {
        Time.timeScale = 1f;
        WorldState.Reset();

        DestroyRoot(PlayerController.Instance);
        DestroyRoot(BaseSingleton.Instance);
        DestroyRoot(UIFade.Instance);
    }

    private static void DestroyRoot(Component persistentComponent)
    {
        if (persistentComponent != null)
        {
            Object.Destroy(persistentComponent.transform.root.gameObject);
        }
    }
}
