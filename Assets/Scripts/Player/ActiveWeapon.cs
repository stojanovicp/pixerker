using System.Collections;
using UnityEngine;

public class ActiveWeapon : Singleton<ActiveWeapon>
{
    public MonoBehaviour CurrentActiveWeapon { get; private set; }
    public WeaponInfo CurrentWeaponInfo { get; private set; }

    private PlayerControls playerControls;
    private float timeBetweenAttacks;

    private bool attackButtonDown, isAttacking = false;


    protected override void Awake() {
        base.Awake();

        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Start()
    {
        playerControls.Combat.Attack.started += _ => StartAttacking();
        playerControls.Combat.Attack.canceled += _ => StopAttacking();

        AttackCooldown();
    }

    private void Update() {
        Attack();
    }

    public void NewWeapon(MonoBehaviour newWeapon)
    {
        CurrentActiveWeapon = newWeapon;
        CurrentWeaponInfo = (CurrentActiveWeapon as IWeapon).GetWeaponInfo();
        timeBetweenAttacks = CurrentWeaponInfo.weaponCooldown;
        AttackCooldown();
    }

    public void WeaponNull()
    {
        CurrentActiveWeapon = null;
        CurrentWeaponInfo = null;
    }

    
    private void AttackCooldown() {
        isAttacking = true;
        StopAllCoroutines();
        StartCoroutine(TimeBetweenAttacksRoutine());
    }

    private IEnumerator TimeBetweenAttacksRoutine()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        isAttacking = false;
    }

    private void StartAttacking()
    {
        attackButtonDown = true;
    }

    private void StopAttacking()
    {
        attackButtonDown = false;
    }

    private void Attack() {
        if (attackButtonDown && !isAttacking && CurrentActiveWeapon) {
            AttackCooldown();
            (CurrentActiveWeapon as IWeapon).Attack();
        }
    }
}
