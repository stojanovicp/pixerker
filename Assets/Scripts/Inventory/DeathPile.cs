using System.Collections.Generic;
using UnityEngine;

// The weapons and gold the player left behind on death. Gold is always returned; weapons only while
// there is a free slot. The pile stays until it is empty.
public class DeathPile : MonoBehaviour
{
    private DeathPileRecord record;

    public void Initialize(DeathPileRecord deathPileRecord)
    {
        record = deathPileRecord;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (record == null || !other.GetComponent<PlayerController>() || PlayerHealth.Instance.IsDead) { return; }

        ReturnGold();
        ReturnWeapons();

        if (record.IsEmpty)
        {
            WorldState.RemoveDeathPile(record);
            record = null;
            Destroy(gameObject);
        }
    }

    private void ReturnGold()
    {
        if (record.Gold == 0) { return; }

        EconomyManager.Instance.AddGold(record.Gold);
        record.Gold = 0;
    }

    private void ReturnWeapons()
    {
        foreach (WeaponInfo weaponInfo in new List<WeaponInfo>(record.Weapons))
        {
            if (ActiveInventory.Instance.TryAddWeapon(weaponInfo))
            {
                record.Weapons.Remove(weaponInfo);
            }
        }
    }
}
