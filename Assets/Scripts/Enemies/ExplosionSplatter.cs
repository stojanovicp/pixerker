using UnityEngine;

public class ExplosionSplatter : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float colliderActiveTime = 0.15f;

    private SpriteFade spriteFade;

    private void Awake()
    {
        spriteFade = GetComponent<SpriteFade>();
    }

    public void SetDamage(int splatterDamage)
    {
        damage = splatterDamage;
    }

    private void Start()
    {
        StartCoroutine(spriteFade.SlowFadeRoutine());

        Invoke(nameof(DisableCollider), colliderActiveTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth) playerHealth.TakeDamage(damage, transform);
    }

    private void DisableCollider()
    {
        GetComponent<CapsuleCollider2D>().enabled = false;
    }
}
