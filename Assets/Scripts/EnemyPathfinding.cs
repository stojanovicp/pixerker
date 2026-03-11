using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private Vector2 moveDir;
    private Rigidbody2D rb;
    //private Animator myAnimator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //myAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        RandomInput();
    }

    private void FixedUpdate()
    {
        Flip();
        Move();
    }
    private void RandomInput()
    {
        //myAnimator.SetFloat("moveX", moveDir.x);
        //myAnimator.SetFloat("moveY", moveDir.y);
    }

    private void Move()
    {
        Vector2 newPosition = rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
    }

    private void Flip()
    {
        var mousePosition = Input.mousePosition;
        var playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (mousePosition.x < playerScreenPosition.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    public void MoveTo(Vector2 targetPosition)
    {
        moveDir = targetPosition;
    }
}
