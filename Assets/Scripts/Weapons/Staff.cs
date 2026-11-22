using UnityEngine;

public class Staff : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private GameObject magicLaser;
    [SerializeField] private Transform magicLaserSpawnPoint;

    private Animator myAnimator;
    readonly int ATTACK_HASH = Animator.StringToHash("Attack");

    void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }

    public void Attack()
    {
        myAnimator.SetTrigger(ATTACK_HASH);
    }

    public void SpawnStaffProjectileAnimEvent()
    {
        GameObject newLaser = Instantiate(magicLaser, magicLaserSpawnPoint.position, ActiveWeapon.Instance.transform.rotation);

        newLaser.GetComponent<MagicLaser>().UpdateLaserRange(weaponInfo.weaponRange);
        newLaser.GetComponent<DamageSource>().Initialize(weaponInfo);
    }
    
    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }
}
