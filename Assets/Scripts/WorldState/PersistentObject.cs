using System.Globalization;
using UnityEngine;

// Marks a scene-placed object whose removal (killed, broken, collected) must survive scene reloads.
// The ID is the object's name plus its start position; it only has to stay the same within one play session.
// Runs before other scripts so a removed object is deactivated before anything else on it initializes.
[DefaultExecutionOrder(-1000)]
public class PersistentObject : MonoBehaviour
{
    private string sceneName;
    private string objectId;
    private bool isExcluded;

    private void Awake()
    {
        sceneName = gameObject.scene.name;
        Vector3 startPosition = transform.position;
        objectId = string.Format(CultureInfo.InvariantCulture, "{0}@{1:F2},{2:F2}", name, startPosition.x, startPosition.y);

        if (WorldState.IsRemoved(sceneName, objectId))
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    public void MarkRemoved()
    {
        if (isExcluded) { return; }

        WorldState.MarkRemoved(sceneName, objectId);
    }

    // For copies spawned at runtime (boss summons): their removal is not recorded.
    public void ExcludeFromWorldState()
    {
        isExcluded = true;
    }
}
