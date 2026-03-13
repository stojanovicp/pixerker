using UnityEngine;
using UnityEngine.PlayerLoop;

public class ActiveWeapon : Singleton<ActiveWeapon>
{
    public MonoBehaviour CurrentActiveWeapon { get; private set; }
    private PlayerControls playerControls;
    private bool attackButtonDown, isAttacking = false;


    protected override void Awake()
    {
        base.Awake();

        playerControls = new PlayerControls();

    }

    private void OnEnable()
    {
        playerControls.Enable();
        
    }
    private void Start()
    {
        playerControls.Combat.Attack.started += _ => StartAttacking();
        playerControls.Combat.Attack.canceled += _ => StopAttacking();
    }
    private void Update()
    {
        Attack();
    }

    public void NewWeapon(MonoBehaviour newWeaopon)
    {
        CurrentActiveWeapon = newWeaopon;
    }
    public void WeaponNull()
    { 
        CurrentActiveWeapon = null; 
    }
    public void ToggleIsAttacking(bool value)
    {
        isAttacking = value;
    }
    private void StartAttacking()
    {
        attackButtonDown = true;
    }
    private void StopAttacking()
    {
        attackButtonDown = false;
    }

    private void Attack()
    {
        if (attackButtonDown && !isAttacking)
        {
            // guard: ensure we actually have an active weapon and it implements IWeapon
            if (CurrentActiveWeapon == null)
                return;

            var weapon = CurrentActiveWeapon as IWeapon;
            if (weapon == null)
            {
                Debug.LogError($"ActiveWeapon.CurrentActiveWeapon does not implement IWeapon. Type: {CurrentActiveWeapon.GetType().Name}");
                WeaponNull();
                return;
            }

            isAttacking = true;
            weapon.Attack();
        }
    }
}
