using UnityEngine;

// Lobs a volley of grapes at the player, following an AttackPattern. The volley starts from the attack
// animation's event; the first grape of each burst lands on the player, the rest scatter around them.
// Attack uses the default pattern; a Boss passes its own patterns through AttackWith.
public class Grape : MonoBehaviour, IEnemy, IPatternAttacker
{
    [SerializeField] private FallingProjectile grapeProjectilePrefab;
    [SerializeField] private AttackPattern attackPattern;

    private Animator myAnimator;
    private SpriteRenderer spriteRenderer;
    private AttackPattern currentPattern;
    private Vector2 volleyTarget;

    readonly int ATTACK_HASH = Animator.StringToHash("Attack");

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentPattern = attackPattern;

        if (grapeProjectilePrefab == null || attackPattern == null)
        {
            Debug.LogError($"{nameof(Grape)} on '{name}': assign Grape Projectile Prefab and Attack Pattern.", this);
            enabled = false;
        }
    }

    public void Attack()
    {
        AttackWith(attackPattern);
    }

    public void AttackWith(AttackPattern pattern)
    {
        if (!enabled || pattern == null) { return; }

        currentPattern = pattern;
        spriteRenderer.flipX = transform.position.x > PlayerController.Instance.transform.position.x;

        // Without an attack animation (e.g. the Black Ghost) the volley starts at once instead of on the anim event.
        if (myAnimator == null)
        {
            SpawnProjectileAnimEvent();
            return;
        }

        myAnimator.SetTrigger(ATTACK_HASH);
    }

    public void SpawnProjectileAnimEvent()
    {
        StartCoroutine(AttackPatternRunner.Run(currentPattern, AimVolley, ThrowGrape));
    }

    private void AimVolley(int burstIndex, int projectileCount)
    {
        volleyTarget = PlayerController.Instance.transform.position;
    }

    private void ThrowGrape(int projectileIndex)
    {
        Vector2 landingPosition = projectileIndex == 0
            ? volleyTarget
            : volleyTarget + Random.insideUnitCircle * currentPattern.targetScatterRadius;

        FallingProjectile grape = Instantiate(grapeProjectilePrefab, transform.position, Quaternion.identity);
        grape.SetImpactDamage(currentPattern.projectileDamage);
        grape.Initialize(landingPosition);
    }
}
