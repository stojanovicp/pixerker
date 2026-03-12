using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private Vector2 moveDir;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Knockback knockback;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        knockback = GetComponent<Knockback>();
    }

    private void FixedUpdate()
    {
        if (knockback.GettingKnockback)
            return;

        Flip();
        Move();
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
