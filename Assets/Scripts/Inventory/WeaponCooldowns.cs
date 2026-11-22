using System.Collections.Generic;
using UnityEngine;

// Ready times are timestamps, so a cooldown keeps running while its weapon is not equipped.
public class WeaponCooldowns
{
    private readonly Dictionary<WeaponInfo, float> readyTimes = new Dictionary<WeaponInfo, float>();

    public bool IsReady(WeaponInfo weaponInfo)
    {
        return GetRemainingTime(weaponInfo) <= 0f;
    }

    public void StartCooldown(WeaponInfo weaponInfo)
    {
        readyTimes[weaponInfo] = Time.time + weaponInfo.weaponCooldown;
    }

    public float GetRemainingFraction(WeaponInfo weaponInfo)
    {
        if (weaponInfo.weaponCooldown <= 0f) { return 0f; }

        return Mathf.Clamp01(GetRemainingTime(weaponInfo) / weaponInfo.weaponCooldown);
    }

    private float GetRemainingTime(WeaponInfo weaponInfo)
    {
        return readyTimes.TryGetValue(weaponInfo, out float readyTime) ? readyTime - Time.time : 0f;
    }
}
