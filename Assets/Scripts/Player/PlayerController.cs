using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Singleton<PlayerController>
{
    public bool FacingLeft { get { return facingLeft; } private set { facingLeft = value; } }
     
    private float moveSpeed = 4f;
    private float dashSpeed = 5f;
    private float dashTime = .13f;
    private float dashCD = 0.3f;
    private float startingMoveSpeed;

    [SerializeField] private TrailRenderer myTrailRenderer;
    [SerializeField] private Transform weaponCollider;
    [SerializeField] private Transform slashAnimSpawnPoint; // <-- added serialized field

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer spriteRenderer;
    private Knockback knockback;

    private bool facingLeft = false;
    private bool isDashing = false;


    protected override void Awake()
    {
        base.Awake();

        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        startingMoveSpeed = moveSpeed;
    }

    private void OnEnable()
    {
        if (playerControls == null)
            playerControls = new PlayerControls();

        playerControls.Enable();
        playerControls.Combat.Dash.performed += OnDashPerformed;
    }

    private void OnDisable()
    {
        // Defensive: input system can call OnDisable during scene transitions when objects may be partially torn down.
        if (playerControls != null)
        {
            playerControls.Combat.Dash.performed -= OnDashPerformed;
            playerControls.Disable();
        }
    }   

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        Flip();
        Move();
    }
    public Transform GetWeaponCollider()
    {
        return weaponCollider;
    }

    // New getter for slash animation spawn point
    public Transform GetSlashAnimSpawnPoint()
    {
        return slashAnimSpawnPoint;
    }

    private void PlayerInput()
    {
        if (playerControls == null)
            return;

        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        if (myAnimator != null)
        {
            myAnimator.SetFloat("moveX", movement.x);
            myAnimator.SetFloat("moveY", movement.y);
        }
    }

    private void Move()
    {
        if(knockback != null && knockback.GettingKnockback)
            return; 
        if (rb == null) 
            return;
        Vector2 newPosition = rb.position + movement * (moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
    }

    private void Flip()
    {
        if (spriteRenderer == null || Camera.main == null) return;

        var mousePosition = Input.mousePosition;
        var playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (mousePosition.x < playerScreenPosition.x)
        {
            spriteRenderer.flipX = true;
            FacingLeft = true;
        }
        else
        {
            spriteRenderer.flipX = false;
            FacingLeft = false;
        }
    }

    private void Dash()
    {
        if(!isDashing)
        {
            isDashing = true;
            moveSpeed *= dashSpeed;
            if (myTrailRenderer != null) myTrailRenderer.emitting = true;
            StartCoroutine(EndDashRoutine());
        }
    }

    private IEnumerator EndDashRoutine()
    {
        yield return new WaitForSeconds(dashTime);
        moveSpeed = startingMoveSpeed;
        if (myTrailRenderer != null) myTrailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCD);
        isDashing = false;
    }

    private void OnDashPerformed(InputAction.CallbackContext ctx) => Dash();
}
