using UnityEngine;

// Lobs a volley of grapes at the player, following an AttackPattern. The volley starts from the attack
// animation's event; the first grape of each burst lands on the player, the rest scatter around them.
public class Grape : MonoBehaviour, IEnemy
{
    [SerializeField] private FallingProjectile grapeProjectilePrefab;
    [SerializeField] private AttackPattern attackPattern;

    private Animator myAnimator;
    private SpriteRenderer spriteRenderer;
    private Vector2 volleyTarget;

    readonly int ATTACK_HASH = Animator.StringToHash("Attack");

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (grapeProjectilePrefab == null || attackPattern == null)
        {
            Debug.LogError($"{nameof(Grape)} on '{name}': assign Grape Projectile Prefab and Attack Pattern.", this);
            enabled = false;
        }
    }

    public void Attack()
    {
        if (!enabled) { return; }

        myAnimator.SetTrigger(ATTACK_HASH);
        spriteRenderer.flipX = transform.position.x > PlayerController.Instance.transform.position.x;
    }

    public void SpawnProjectileAnimEvent()
    {
        StartCoroutine(AttackPatternRunner.Run(attackPattern, AimVolley, ThrowGrape));
    }

    private void AimVolley(int burstIndex)
    {
        volleyTarget = PlayerController.Instance.transform.position;
    }

    private void ThrowGrape(int projectileIndex)
    {
        Vector2 landingPosition = projectileIndex == 0
            ? volleyTarget
            : volleyTarget + Random.insideUnitCircle * attackPattern.targetScatterRadius;

        FallingProjectile grape = Instantiate(grapeProjectilePrefab, transform.position, Quaternion.identity);
        grape.Initialize(landingPosition);
    }
}
