using UnityEngine;

public class DroppedWeaponRecord
{
    public string SceneName { get; }
    public WeaponInfo WeaponInfo { get; }
    public Vector2 Position { get; }

    public DroppedWeaponRecord(string sceneName, WeaponInfo weaponInfo, Vector2 position)
    {
        SceneName = sceneName;
        WeaponInfo = weaponInfo;
        Position = position;
    }
}
