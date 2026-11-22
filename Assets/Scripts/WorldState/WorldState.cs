using System;
using System.Collections.Generic;
using UnityEngine;

// In-memory world state for one play session, keyed by scene name. Nothing is saved to disk.
public static class WorldState
{
    // The Vector2 is where a freshly dropped item pops out from; it arcs from there to the record's position.
    public static event Action<DroppedWeaponRecord, Vector2> DroppedWeaponAdded;
    public static event Action<DroppedCoinRecord, Vector2> DroppedCoinAdded;

    private static readonly Dictionary<string, HashSet<string>> removedObjectIds = new Dictionary<string, HashSet<string>>();
    private static readonly Dictionary<string, List<DroppedWeaponRecord>> droppedWeapons = new Dictionary<string, List<DroppedWeaponRecord>>();
    private static readonly Dictionary<string, List<DroppedCoinRecord>> droppedCoins = new Dictionary<string, List<DroppedCoinRecord>>();

    // Also covers Play Mode without domain reload, where static fields would otherwise survive.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay()
    {
        DroppedWeaponAdded = null;
        DroppedCoinAdded = null;
        Reset();
    }

    public static void Reset()
    {
        removedObjectIds.Clear();
        droppedWeapons.Clear();
        droppedCoins.Clear();
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
        return GetRecords(droppedWeapons, sceneName);
    }

    public static void AddDroppedWeapon(DroppedWeaponRecord record, Vector2 popOrigin)
    {
        GetOrCreate(droppedWeapons, record.SceneName).Add(record);
        DroppedWeaponAdded?.Invoke(record, popOrigin);
    }

    public static void RemoveDroppedWeapon(DroppedWeaponRecord record)
    {
        RemoveRecord(droppedWeapons, record.SceneName, record);
    }

    public static IReadOnlyList<DroppedCoinRecord> GetDroppedCoins(string sceneName)
    {
        return GetRecords(droppedCoins, sceneName);
    }

    public static void AddDroppedCoin(DroppedCoinRecord record, Vector2 popOrigin)
    {
        GetOrCreate(droppedCoins, record.SceneName).Add(record);
        DroppedCoinAdded?.Invoke(record, popOrigin);
    }

    public static void RemoveDroppedCoin(DroppedCoinRecord record)
    {
        RemoveRecord(droppedCoins, record.SceneName, record);
    }

    private static IReadOnlyList<TRecord> GetRecords<TRecord>(Dictionary<string, List<TRecord>> recordsByScene, string sceneName)
    {
        return recordsByScene.TryGetValue(sceneName, out List<TRecord> records) ? records : Array.Empty<TRecord>();
    }

    private static void RemoveRecord<TRecord>(Dictionary<string, List<TRecord>> recordsByScene, string sceneName, TRecord record)
    {
        if (recordsByScene.TryGetValue(sceneName, out List<TRecord> records))
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
