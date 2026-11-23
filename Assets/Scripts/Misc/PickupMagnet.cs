using UnityEngine;

// Pulls a pickup toward the living player once they are within range. Disable the component to turn it off.
// The pull speed grows every frame the player stays in range and resets when they leave.
[RequireComponent(typeof(Rigidbody2D))]
public class PickupMagnet : MonoBehaviour
{
    [SerializeField] private float pickUpDistance = 4f;
    [SerializeField] private float accelerationRate = 1f;
    [SerializeField] private float moveSpeed = 4f;

    private Rigidbody2D rb;
    private Vector3 moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnDisable()
    {
        moveDirection = Vector3.zero;
        rb.linearVelocity = Vector2.zero;
    }

    private void Update()
    {
        if (PlayerHealth.Instance.IsDead)
        {
            moveDirection = Vector3.zero;
            return;
        }

        Vector3 playerPosition = PlayerController.Instance.transform.position;

        if (Vector3.Distance(transform.position, playerPosition) < pickUpDistance)
        {
            moveDirection = (playerPosition - transform.position).normalized;
            moveSpeed += accelerationRate;
        }
        else
        {
            moveDirection = Vector3.zero;
            moveSpeed = 0f;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * moveSpeed * Time.deltaTime;
    }
}
