using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Runs the death-to-respawn sequence. The Player object is kept alive and reset, never destroyed.
public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private string respawnSceneName = "Scene_1";
    [SerializeField] private float deathAnimationTime = 2f;
    [SerializeField] private float fadeOutTime = 1f;

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
        LeaveDeathPile();
        StartCoroutine(RespawnRoutine());
    }

    // The pile keeps every droppable weapon and half the gold (rounded down); the other half is lost.
    private void LeaveDeathPile()
    {
        List<WeaponInfo> weapons = ActiveInventory.Instance.TakeDroppableWeapons();
        int pileGold = EconomyManager.Instance.TakeAllGold() / 2;

        if (weapons.Count == 0 && pileGold == 0) { return; }

        string sceneName = SceneManager.GetActiveScene().name;
        WorldState.AddDeathPile(new DeathPileRecord(sceneName, transform.position, weapons, pileGold));
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
