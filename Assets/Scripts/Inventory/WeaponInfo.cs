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
    public DamageFlags damageFlags;
    [Tooltip("Share of the damage dealt that heals the player (0 = none).")]
    [Range(0f, 1f)] public float lifeStealFraction;
}
