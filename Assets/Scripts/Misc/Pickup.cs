using System.Collections;
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


    private Vector3 moveDir;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StartCoroutine(AnimCurveSpawnRoutine());
    }

    private void Update()
    {
        if (PlayerController.Instance == null) { return; }

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
        if(collision.gameObject.GetComponent<PlayerController>())
        {
            Destroy(gameObject);
            DetectPickupType();
        }
    }

    private IEnumerator AnimCurveSpawnRoutine()
    {
        Vector2 startPoint = transform.position;

        float randomX = transform.position.x + UnityEngine.Random.Range(-2f, 2f);
        float randomY = transform.position.y + UnityEngine.Random.Range(-1f, 1f);

        Vector2 endPoint = new Vector2(randomX,randomY);

        float timePassed = 0f;

        while(timePassed < popDuration)
        {
            timePassed += Time.deltaTime;

            float linearT = timePassed / popDuration;
            float heightT = animationCurve.Evaluate(linearT);
            float height = Mathf.Lerp(0f, heightY, heightT);

            transform.position = Vector2.Lerp(startPoint, endPoint, linearT) + new Vector2(0f, height);

            yield return null;
        }
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
