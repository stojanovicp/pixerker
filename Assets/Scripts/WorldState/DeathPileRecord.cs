using System.Collections.Generic;
using UnityEngine;

// What the player left behind on death. Weapons and gold are removed as the player collects them.
public class DeathPileRecord
{
    public string SceneName { get; }
    public Vector2 Position { get; }
    public List<WeaponInfo> Weapons { get; }
    public int Gold { get; set; }

    public bool IsEmpty => Weapons.Count == 0 && Gold == 0;

    public DeathPileRecord(string sceneName, Vector2 position, List<WeaponInfo> weapons, int gold)
    {
        SceneName = sceneName;
        Position = position;
        Weapons = weapons;
        Gold = gold;
    }
}
