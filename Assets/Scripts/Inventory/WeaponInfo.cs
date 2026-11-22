using UnityEngine;

[CreateAssetMenu(menuName = "New Weapon")]
public class WeaponInfo : ScriptableObject
{
    public GameObject weaponPrefab;
    public float weaponCooldown;
    public int weaponDamage;
    public float weaponRange;
    public float knockbackForce;
    public WeaponAimMode aimMode;
    public Sprite icon;
    public bool canBeDropped = true;
}
