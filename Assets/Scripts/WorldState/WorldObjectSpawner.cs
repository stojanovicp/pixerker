using UnityEngine;
using UnityEngine.SceneManagement;

// Recreates runtime world objects (dropped weapons, death piles) from WorldState when their scene loads.
// Lives on the persistent Managers object. It subscribes in Start, not OnEnable: the Managers copy in each
// newly loaded scene is destroyed before Start runs, so only the persistent spawner ever spawns.
public class WorldObjectSpawner : MonoBehaviour
{
    [SerializeField] private WeaponPickup weaponPickupPrefab;
    [SerializeField] private DeathPile deathPilePrefab;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        WorldState.DroppedWeaponAdded += OnDroppedWeaponAdded;
        WorldState.DeathPileAdded += OnDeathPileAdded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        WorldState.DroppedWeaponAdded -= OnDroppedWeaponAdded;
        WorldState.DeathPileAdded -= OnDeathPileAdded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (DroppedWeaponRecord record in WorldState.GetDroppedWeapons(scene.name))
        {
            SpawnDroppedWeapon(record, scene, true);
        }

        foreach (DeathPileRecord record in WorldState.GetDeathPiles(scene.name))
        {
            SpawnDeathPile(record, scene);
        }
    }

    // A fresh drop lands on the player, so it only arms after the player steps off it.
    private void OnDroppedWeaponAdded(DroppedWeaponRecord record)
    {
        Scene activeScene = SceneManager.GetActiveScene();

        if (record.SceneName != activeScene.name) { return; }

        SpawnDroppedWeapon(record, activeScene, false);
    }

    private void OnDeathPileAdded(DeathPileRecord record)
    {
        Scene activeScene = SceneManager.GetActiveScene();

        if (record.SceneName != activeScene.name) { return; }

        SpawnDeathPile(record, activeScene);
    }

    private void SpawnDroppedWeapon(DroppedWeaponRecord record, Scene scene, bool armImmediately)
    {
        WeaponPickup pickup = Instantiate(weaponPickupPrefab, record.Position, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(pickup.gameObject, scene);
        pickup.InitializeAsDropped(record, armImmediately);
    }

    private void SpawnDeathPile(DeathPileRecord record, Scene scene)
    {
        DeathPile deathPile = Instantiate(deathPilePrefab, record.Position, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(deathPile.gameObject, scene);
        deathPile.Initialize(record);
    }
}
