using UnityEngine;
using UnityEngine.Serialization;

// A coin or globe. Pops out when spawned; the PickupMagnet on the same object pulls it to the player.
public class Pickup : MonoBehaviour
{
    private enum PickUpType
    {
        GoldCoin,
        StaminaGlobe,
        HealthGlobe
    }

    [SerializeField] private PickUpType pickUpType;
    [Tooltip("Gold, health or stamina given; rolled once per pickup between min and max (inclusive).")]
    [FormerlySerializedAs("amount")]
    [Min(0)] [SerializeField] private int minAmount = 1;
    [Min(0)] [SerializeField] private int maxAmount = 1;
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private float heightY = 1.5f;
    [SerializeField] private float popDuration = 1f;
    [Tooltip("Half-size of the area a normal drop pops out to (x, y).")]
    [SerializeField] private Vector2 randomPopRange = new Vector2(2f, 1f);

    private DroppedCoinRecord droppedRecord;
    private int amount;

    private void OnValidate()
    {
        maxAmount = Mathf.Max(maxAmount, minAmount);
    }

    private void Awake()
    {
        amount = Random.Range(minAmount, maxAmount + 1);
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
