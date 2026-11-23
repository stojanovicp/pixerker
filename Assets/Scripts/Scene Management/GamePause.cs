using UnityEngine;

// Freezes and unfreezes gameplay. While paused, time stands still and player input is ignored.
// Used by the pause menu and the victory screen.
public static class GamePause
{
    public static bool IsPaused { get; private set; }

    public static void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
    }

    public static void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
    }
}
