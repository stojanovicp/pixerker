using UnityEngine;
using UnityEngine.SceneManagement;

// Recreates runtime world objects (dropped weapons) from WorldState when their scene loads.
// Lives on the persistent Managers object. It subscribes in Start, not OnEnable: the Managers copy in each
// newly loaded scene is destroyed before Start runs, so only the persistent spawner ever spawns.
public class WorldObjectSpawner : MonoBehaviour
{
    [SerializeField] private WeaponPickup weaponPickupPrefab;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        WorldState.DroppedWeaponAdded += OnDroppedWeaponAdded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        WorldState.DroppedWeaponAdded -= OnDroppedWeaponAdded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (DroppedWeaponRecord record in WorldState.GetDroppedWeapons(scene.name))
        {
            SpawnDroppedWeapon(record, scene, true);
        }
    }

    // A fresh drop lands on the player, so it only arms after the player steps off it.
    private void OnDroppedWeaponAdded(DroppedWeaponRecord record)
    {
        Scene activeScene = SceneManager.GetActiveScene();

        if (record.SceneName != activeScene.name) { return; }

        SpawnDroppedWeapon(record, activeScene, false);
    }

    private void SpawnDroppedWeapon(DroppedWeaponRecord record, Scene scene, bool armImmediately)
    {
        WeaponPickup pickup = Instantiate(weaponPickupPrefab, record.Position, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(pickup.gameObject, scene);
        pickup.InitializeAsDropped(record, armImmediately);
    }
}
