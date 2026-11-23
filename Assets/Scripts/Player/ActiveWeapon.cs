using UnityEngine;

public class ActiveWeapon : Singleton<ActiveWeapon>
{
    public MonoBehaviour CurrentActiveWeapon { get; private set; }
    public WeaponInfo CurrentWeaponInfo { get; private set; }

    private readonly WeaponCooldowns weaponCooldowns = new WeaponCooldowns();
    private PlayerControls playerControls;
    private bool attackButtonDown;

    protected override void Awake() {
        base.Awake();

        if (Instance != this) { return; }

        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls?.Enable();
    }

    private void OnDisable()
    {
        playerControls?.Disable();
    }

    private void Start()
    {
        playerControls.Combat.Attack.started += _ => StartAttacking();
        playerControls.Combat.Attack.canceled += _ => StopAttacking();
    }

    private void Update() {
        Attack();
    }

    public void NewWeapon(MonoBehaviour newWeapon)
    {
        CurrentActiveWeapon = newWeapon;
        CurrentWeaponInfo = (CurrentActiveWeapon as IWeapon).GetWeaponInfo();
    }

    public void WeaponNull()
    {
        CurrentActiveWeapon = null;
        CurrentWeaponInfo = null;
    }

    public float GetCooldownRemainingFraction(WeaponInfo weaponInfo)
    {
        return weaponCooldowns.GetRemainingFraction(weaponInfo);
    }

    private void StartAttacking()
    {
        if (GamePause.IsPaused) { return; }

        attackButtonDown = true;
    }

    private void StopAttacking()
    {
        attackButtonDown = false;
    }

    private void Attack() {
        if (!attackButtonDown || CurrentActiveWeapon == null || GamePause.IsPaused) { return; }
        if (!weaponCooldowns.IsReady(CurrentWeaponInfo)) { return; }

        weaponCooldowns.StartCooldown(CurrentWeaponInfo);
        (CurrentActiveWeapon as IWeapon).Attack();
    }
}
