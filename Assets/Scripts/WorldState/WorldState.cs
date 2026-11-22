using System;
using System.Collections.Generic;
using UnityEngine;

// In-memory world state for one play session, keyed by scene name. Nothing is saved to disk.
public static class WorldState
{
    public static event Action<DroppedWeaponRecord> DroppedWeaponAdded;
    public static event Action<DeathPileRecord> DeathPileAdded;

    private static readonly Dictionary<string, HashSet<string>> removedObjectIds = new Dictionary<string, HashSet<string>>();
    private static readonly Dictionary<string, List<DroppedWeaponRecord>> droppedWeapons = new Dictionary<string, List<DroppedWeaponRecord>>();
    private static readonly Dictionary<string, List<DeathPileRecord>> deathPiles = new Dictionary<string, List<DeathPileRecord>>();

    // Also covers Play Mode without domain reload, where static fields would otherwise survive.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay()
    {
        DroppedWeaponAdded = null;
        DeathPileAdded = null;
        Reset();
    }

    public static void Reset()
    {
        removedObjectIds.Clear();
        droppedWeapons.Clear();
        deathPiles.Clear();
    }

    public static bool IsRemoved(string sceneName, string objectId)
    {
        return removedObjectIds.TryGetValue(sceneName, out HashSet<string> ids) && ids.Contains(objectId);
    }

    public static void MarkRemoved(string sceneName, string objectId)
    {
        GetOrCreate(removedObjectIds, sceneName).Add(objectId);
    }

    public static IReadOnlyList<DroppedWeaponRecord> GetDroppedWeapons(string sceneName)
    {
        return droppedWeapons.TryGetValue(sceneName, out List<DroppedWeaponRecord> records)
            ? records
            : Array.Empty<DroppedWeaponRecord>();
    }

    public static void AddDroppedWeapon(DroppedWeaponRecord record)
    {
        GetOrCreate(droppedWeapons, record.SceneName).Add(record);
        DroppedWeaponAdded?.Invoke(record);
    }

    public static void RemoveDroppedWeapon(DroppedWeaponRecord record)
    {
        if (droppedWeapons.TryGetValue(record.SceneName, out List<DroppedWeaponRecord> records))
        {
            records.Remove(record);
        }
    }

    public static IReadOnlyList<DeathPileRecord> GetDeathPiles(string sceneName)
    {
        return deathPiles.TryGetValue(sceneName, out List<DeathPileRecord> records)
            ? records
            : Array.Empty<DeathPileRecord>();
    }

    public static void AddDeathPile(DeathPileRecord record)
    {
        GetOrCreate(deathPiles, record.SceneName).Add(record);
        DeathPileAdded?.Invoke(record);
    }

    public static void RemoveDeathPile(DeathPileRecord record)
    {
        if (deathPiles.TryGetValue(record.SceneName, out List<DeathPileRecord> records))
        {
            records.Remove(record);
        }
    }

    private static TValue GetOrCreate<TValue>(Dictionary<string, TValue> dictionary, string key) where TValue : new()
    {
        if (!dictionary.TryGetValue(key, out TValue value))
        {
            value = new TValue();
            dictionary[key] = value;
        }

        return value;
    }
}
