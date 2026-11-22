using UnityEngine;
using UnityEngine.SceneManagement;

// Recreates runtime world objects (dropped weapons, dropped coins) from WorldState when their scene loads,
// and spawns fresh drops at their pop origin so they arc to their recorded position.
// Lives on the persistent Managers object. It subscribes in Start, not OnEnable: the Managers copy in each
// newly loaded scene is destroyed before Start runs, so only the persistent spawner ever spawns.
public class WorldObjectSpawner : MonoBehaviour
{
    [SerializeField] private WeaponPickup weaponPickupPrefab;
    [SerializeField] private Pickup goldCoinPrefab;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        WorldState.DroppedWeaponAdded += OnDroppedWeaponAdded;
        WorldState.DroppedCoinAdded += OnDroppedCoinAdded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        WorldState.DroppedWeaponAdded -= OnDroppedWeaponAdded;
        WorldState.DroppedCoinAdded -= OnDroppedCoinAdded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (DroppedWeaponRecord record in WorldState.GetDroppedWeapons(scene.name))
        {
            SpawnDroppedWeapon(record, record.Position, scene, true);
        }

        foreach (DroppedCoinRecord record in WorldState.GetDroppedCoins(scene.name))
        {
            SpawnDroppedCoin(record, record.Position, scene);
        }
    }

    // A fresh drop may land on the player, so it only arms after the player steps off it.
    private void OnDroppedWeaponAdded(DroppedWeaponRecord record, Vector2 popOrigin)
    {
        if (!IsInActiveScene(record.SceneName)) { return; }

        SpawnDroppedWeapon(record, popOrigin, SceneManager.GetActiveScene(), false);
    }

    private void OnDroppedCoinAdded(DroppedCoinRecord record, Vector2 popOrigin)
    {
        if (!IsInActiveScene(record.SceneName)) { return; }

        SpawnDroppedCoin(record, popOrigin, SceneManager.GetActiveScene());
    }

    private bool IsInActiveScene(string sceneName)
    {
        return sceneName == SceneManager.GetActiveScene().name;
    }

    private void SpawnDroppedWeapon(DroppedWeaponRecord record, Vector2 spawnPosition, Scene scene, bool armImmediately)
    {
        WeaponPickup pickup = Instantiate(weaponPickupPrefab, spawnPosition, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(pickup.gameObject, scene);
        pickup.InitializeAsDropped(record, armImmediately);
    }

    private void SpawnDroppedCoin(DroppedCoinRecord record, Vector2 spawnPosition, Scene scene)
    {
        Pickup coin = Instantiate(goldCoinPrefab, spawnPosition, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(coin.gameObject, scene);
        coin.InitializeAsDropped(record);
    }
}
