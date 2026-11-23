using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    public bool FacingLeft { get { return facingLeft; } }

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float dashSpeed = 5f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.25f;
    [SerializeField] private TrailRenderer myTrailRenderer;
    [SerializeField] private Transform weaponCollider;
    [SerializeField] private Transform slashAnimationSpawnPoint;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;
    private Knockback knockback;
    private float startingMoveSpeed;

    private bool facingLeft = false;
    private bool isDashing = false;

    readonly int MOVE_X_HASH = Animator.StringToHash("moveX");
    readonly int MOVE_Y_HASH = Animator.StringToHash("moveY");

    protected override void Awake() {
        base.Awake();

        if (Instance != this) { return; }

        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRender = GetComponent<SpriteRenderer>();
        knockback = GetComponent<Knockback>();
    }

    private void Start() {
        playerControls.Combat.Dash.performed += _ => Dash();

        startingMoveSpeed = moveSpeed;

        ActiveInventory.Instance.EquipStartingWeapon();
    }

    private void OnEnable() {
        playerControls?.Enable();
    }

    private void OnDisable() {
        playerControls?.Disable();
    }

    private void Update() {
        if (GamePause.IsPaused) { return; }

        PlayerInput();
        AdjustPlayerFacingDirection();
    }

    private void FixedUpdate() {
        Move();
    }

    public Transform GetWeaponCollider()
    {
        return weaponCollider;
    }

    public Transform GetSlashAnimationSpawnPoint()
    {
        return slashAnimationSpawnPoint;
    }

    private void PlayerInput() {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        myAnimator.SetFloat(MOVE_X_HASH, movement.x);
        myAnimator.SetFloat(MOVE_Y_HASH, movement.y);
    }

    private void Move() {
        if(knockback.GettingKnockedBack || PlayerHealth.Instance.IsDead) { return; }
        
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    // Clears dash state, trail and leftover velocity in case the player died mid-dash or mid-knockback.
    public void ResetForRespawn() {
        StopAllCoroutines();
        isDashing = false;
        moveSpeed = startingMoveSpeed;
        myTrailRenderer.emitting = false;
        myTrailRenderer.Clear();
        movement = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }

    private void AdjustPlayerFacingDirection() {
        if (PlayerHealth.Instance.IsDead) { return; }

        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (MouseInput.ScreenPosition.x < playerScreenPoint.x) {
            mySpriteRender.flipX = true;
            facingLeft = true;
        } else {
            mySpriteRender.flipX = false;
            facingLeft = false;
        }
    }

    private void Dash() {
        if (PlayerHealth.Instance.IsDead || GamePause.IsPaused) { return; }

        if (!isDashing && Stamina.Instance.CurrentStamina > 0) {
            Stamina.Instance.UseStamina();
            isDashing = true;
            moveSpeed *= dashSpeed;
            myTrailRenderer.emitting = true;
            StartCoroutine(EndDashRoutine());
        }
    }

    private IEnumerator EndDashRoutine() {
        yield return new WaitForSeconds(dashDuration);
        moveSpeed = startingMoveSpeed;
        myTrailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCooldown);
        isDashing = false;
    }
}
