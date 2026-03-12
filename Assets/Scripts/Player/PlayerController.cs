using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool FacingLeft { get { return facingLeft; } set { facingLeft = value; } }
    public static PlayerController Instance;
     
    [SerializeField] private float moveSpeed = 4f;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer spriteRenderer;

    private bool facingLeft = false;


    private void Awake()
    {
        Instance = this; 
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
}
