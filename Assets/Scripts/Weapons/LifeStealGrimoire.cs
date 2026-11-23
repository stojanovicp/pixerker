using UnityEngine;

// Fires one life-stealing bolt toward the mouse. Execute and life steal come from the WeaponInfo via DamageSource.
// Like the Staff, Attack only starts the swing; the bolt is cast on the animation event.
public class LifeStealGrimoire : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private Projectile boltPrefab;

    private Animator myAnimator;

    readonly int ATTACK_HASH = Animator.StringToHash("Attack");

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }

    public void Attack()
    {
        myAnimator.SetTrigger(ATTACK_HASH);
    }

    public void CastBoltAnimEvent()
    {
        Vector2 spawnPosition = transform.position;
        Vector2 direction = (MouseInput.WorldPosition - spawnPosition).normalized;

        if (direction == Vector2.zero) { direction = Vector2.right; }

        Projectile bolt = Instantiate(boltPrefab, spawnPosition, Quaternion.identity);
        bolt.transform.right = direction;
        bolt.UpdateProjectileRange(weaponInfo.weaponRange);
        bolt.GetComponent<DamageSource>().Initialize(weaponInfo);
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }
}
