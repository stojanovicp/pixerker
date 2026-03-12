using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool FacingLeft { get { return facingLeft; } private set { facingLeft = value; } }
    public static PlayerController Instance;
     
    private float moveSpeed = 4f;
    private float dashSpeed = 5f;
    private float dashTime = .13f;
    private float dashCD = 0.3f;
    private float startingMoveSpeed;

    [SerializeField] private TrailRenderer myTrailRenderer;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer spriteRenderer;

    private bool facingLeft = false;
    private bool isDashing = false;


    private void Awake()
    {
        Instance = this; 
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        playerControls.Combat.Dash.performed += _ => Dash();
        startingMoveSpeed = moveSpeed;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
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
    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
    }

    private void Move()
    {
        Vector2 newPosition = rb.position + movement * (moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
    }

    private void Flip()
    {
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
            myTrailRenderer.emitting = true;
            StartCoroutine(EndDashRoutine());
        }
    }

    private IEnumerator EndDashRoutine()
    {
        yield return new WaitForSeconds(dashTime);
        moveSpeed = startingMoveSpeed;
        myTrailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCD);
        isDashing = false;
    }
}
