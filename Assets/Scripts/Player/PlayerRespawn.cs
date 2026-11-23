using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Runs the death-to-respawn sequence. The Player object is kept alive and reset, never destroyed.
public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private string respawnSceneName = "Scene_1";
    [SerializeField] private float deathAnimationTime = 2f;
    [SerializeField] private float fadeOutTime = 1f;

    [Header("Death drops")]
    [SerializeField] private float dropScatterRadius = 1.5f;
    [Min(1)] [SerializeField] private int maxDeathCoins = 8;

    private PlayerHealth playerHealth;
    private PlayerController playerController;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        playerHealth.Died += OnPlayerDied;
    }

    private void OnDisable()
    {
        playerHealth.Died -= OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        ActiveInventory.Instance.UnequipWeapon();
        DropBelongings();
        StartCoroutine(RespawnRoutine());
    }

    // Droppable weapons and half the gold (rounded down) scatter around the death position as persistent
    // pickups; the other half of the gold is lost.
    private void DropBelongings()
    {
        Vector2 deathPosition = transform.position;
        string sceneName = SceneManager.GetActiveScene().name;

        foreach (WeaponInfo weaponInfo in ActiveInventory.Instance.TakeDroppableWeapons())
        {
            DroppedWeaponRecord record = new DroppedWeaponRecord(sceneName, weaponInfo, GetScatterPoint(deathPosition), true);
            WorldState.AddDroppedWeapon(record, deathPosition);
        }

        DropGold(EconomyManager.Instance.TakeAllGold() / 2, deathPosition, sceneName);
    }

    // Splits the gold over at most maxDeathCoins coins; the first coins carry the remainder.
    private void DropGold(int gold, Vector2 deathPosition, string sceneName)
    {
        if (gold <= 0) { return; }

        int coinCount = Mathf.Min(maxDeathCoins, gold);
        int baseAmount = gold / coinCount;
        int remainder = gold % coinCount;

        for (int i = 0; i < coinCount; i++)
        {
            int coinAmount = baseAmount + (i < remainder ? 1 : 0);
            DroppedCoinRecord record = new DroppedCoinRecord(sceneName, GetScatterPoint(deathPosition), coinAmount);
            WorldState.AddDroppedCoin(record, deathPosition);
        }
    }

    private Vector2 GetScatterPoint(Vector2 center)
    {
        return center + Random.insideUnitCircle * dropScatterRadius;
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(deathAnimationTime);

        UIFade.Instance.FadeToBlack();
        yield return new WaitForSeconds(fadeOutTime);

        ResetPlayer();

        SceneManagement.Instance.SetTransitionName(SceneManagement.RESPAWN_TRANSITION_NAME);
        SceneManager.LoadScene(respawnSceneName);
    }

    // Health must be reset first: the inventory refuses to equip while the player is dead.
    private void ResetPlayer()
    {
        playerHealth.ResetForRespawn();
        playerController.ResetForRespawn();
        Stamina.Instance.ResetForRespawn();
        ActiveInventory.Instance.EquipStartingWeapon();
    }
}
