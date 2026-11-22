using UnityEngine;
using UnityEngine.Serialization;

public class Pickup : MonoBehaviour
{
    private enum PickUpType
    {
        GoldCoin,
        StaminaGlobe,
        HealthGlobe
    }

    [SerializeField] private PickUpType pickUpType;
    [SerializeField] private int amount = 1;
    [SerializeField] private float pickUpDistance = 4f;
    [FormerlySerializedAs("accelartionRate")]
    [SerializeField] private float accelerationRate = 0.6f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private float heightY = 1.5f;
    [SerializeField] private float popDuration = 1f;
    [Tooltip("Half-size of the area a normal drop pops out to (x, y).")]
    [SerializeField] private Vector2 randomPopRange = new Vector2(2f, 1f);

    private Vector3 moveDir;
    private Rigidbody2D rb;
    private DroppedCoinRecord droppedRecord;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Persistent coin (dropped on player death): its amount and landing position come from the record.
    public void InitializeAsDropped(DroppedCoinRecord record)
    {
        droppedRecord = record;
        amount = record.Amount;
    }

    private void Start()
    {
        Vector2 startPoint = transform.position;
        Vector2 endPoint = droppedRecord != null ? droppedRecord.Position : GetRandomPopPoint(startPoint);

        if (endPoint != startPoint)
        {
            StartCoroutine(ArcMovement.Move(transform, startPoint, endPoint, animationCurve, heightY, popDuration));
        }
    }

    private void Update()
    {
        if (PlayerHealth.Instance.IsDead)
        {
            moveDir = Vector3.zero;
            return;
        }

        Vector3 playerPos = PlayerController.Instance.transform.position;

        if(Vector3.Distance(transform.position, playerPos) < pickUpDistance)
        {
            moveDir = (playerPos - transform.position).normalized;
            moveSpeed += accelerationRate;
        }
        else
        {
            moveDir = Vector3.zero;
            moveSpeed = 0f;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveDir * moveSpeed * Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>() && !PlayerHealth.Instance.IsDead)
        {
            Destroy(gameObject);

            if (droppedRecord != null)
            {
                WorldState.RemoveDroppedCoin(droppedRecord);
            }

            DetectPickupType();
        }
    }

    private Vector2 GetRandomPopPoint(Vector2 origin)
    {
        return origin + new Vector2(
            Random.Range(-randomPopRange.x, randomPopRange.x),
            Random.Range(-randomPopRange.y, randomPopRange.y));
    }

    private void DetectPickupType()
    {
        switch(pickUpType)
        {
            case PickUpType.GoldCoin:
                EconomyManager.Instance.AddGold(amount);
                break;

            case PickUpType.HealthGlobe:
                PlayerHealth.Instance.HealPlayer(amount);
                break;

            case PickUpType.StaminaGlobe:
                Stamina.Instance.RefreshStamina(amount);
                break;
            
            default:
                break;
        }
    }
}
