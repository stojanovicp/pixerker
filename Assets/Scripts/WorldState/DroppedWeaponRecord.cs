using UnityEngine;

public class DroppedWeaponRecord
{
    public string SceneName { get; }
    public WeaponInfo WeaponInfo { get; }
    public Vector2 Position { get; }

    // Death drops attract to the player (they can land out of reach); Q drops must stay where the player put them.
    public bool AttractsToPlayer { get; }

    public DroppedWeaponRecord(string sceneName, WeaponInfo weaponInfo, Vector2 position, bool attractsToPlayer)
    {
        SceneName = sceneName;
        WeaponInfo = weaponInfo;
        Position = position;
        AttractsToPlayer = attractsToPlayer;
    }
}
