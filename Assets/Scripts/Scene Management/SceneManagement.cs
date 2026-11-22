using UnityEngine;

public class SceneManagement : Singleton<SceneManagement>
{
    // Must match the Transition Name of the Respawn Point's AreaEntrance in the respawn scene.
    public const string RESPAWN_TRANSITION_NAME = "Respawn";

    public string SceneTransitionName { get; private set; }

    public void SetTransitionName(string sceneTransitionName) {
        this.SceneTransitionName = sceneTransitionName;
    }
}
