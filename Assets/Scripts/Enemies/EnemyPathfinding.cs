using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private EnemyStats stats;

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Knockback knockback;
    private SpriteRenderer spriteRenderer;
    private float moveSpeed;

    private void Awake() {
        knockback = GetComponent<Knockback>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        moveSpeed = stats.moveSpeed;
    }

    private void FixedUpdate() {
        if (knockback.GettingKnockedBack) { return; }

        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));

        if (moveDir.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveDir.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    public void MoveInDirection(Vector2 direction) {
        moveDir = direction;
    }

    public void StopMoving()
    {
        moveDir = Vector3.zero;
    }

    // 0 or less restores the speed from EnemyStats (bosses change speed per phase).
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed > 0f ? speed : stats.moveSpeed;
    }
}
