using UnityEngine;

// Lobs a bomb toward the mouse; the landing point is clamped to the weapon's range around the player.
public class LightningBomb : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private FallingProjectile bombProjectilePrefab;

    public void Attack()
    {
        Vector2 playerPosition = PlayerController.Instance.transform.position;
        Vector2 towardMouse = MouseInput.WorldPosition - playerPosition;
        Vector2 landingPosition = playerPosition + Vector2.ClampMagnitude(towardMouse, weaponInfo.weaponRange);

        FallingProjectile bomb = Instantiate(bombProjectilePrefab, transform.position, Quaternion.identity);
        bomb.GetComponent<BlastDamage>().Initialize(weaponInfo);
        bomb.Initialize(landingPosition);
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }
}
