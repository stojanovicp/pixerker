using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Raised when the enemy starts chasing the player (leaves Roam). Bosses use it to engage.
    public event Action PlayerDetected;

    [SerializeField] private EnemyStats stats;
    [SerializeField] private MonoBehaviour enemyType;

    public EnemyStats Stats => stats;
    public int ContactDamage => stats.contactDamage;
    public bool CanAttack => attack != null && stats.attackRange > 0f;
    public bool IsPlayerAlive => !PlayerHealth.Instance.IsDead;
    public float DistanceToPlayer => Vector2.Distance(transform.position, PlayerController.Instance.transform.position);

    public EnemyRoamState RoamState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }

    private readonly StateMachine stateMachine = new StateMachine();
    private EnemyPathfinding enemyPathfinding;
    private IEnemy attack;
    private float nextAttackTime;

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        attack = enemyType as IEnemy;

        if (stats.attackRange > 0f && attack == null)
        {
            Debug.LogError($"{nameof(EnemyAI)} on '{name}': Attack Range is above 0 but Enemy Type is not an {nameof(IEnemy)}. This enemy will not attack.", this);
        }

        RoamState = new EnemyRoamState(this);
        ChaseState = new EnemyChaseState(this);
        AttackState = new EnemyAttackState(this);
    }

    private void Start()
    {
        stateMachine.ChangeState(RoamState);
    }

    private void Update()
    {
        stateMachine.Tick();
    }

    public void ChangeState(IState nextState)
    {
        bool detectsPlayer = stateMachine.CurrentState == RoamState && nextState == ChaseState;

        stateMachine.ChangeState(nextState);

        if (detectsPlayer)
        {
            PlayerDetected?.Invoke();
        }
    }

    public void MoveInDirection(Vector2 direction)
    {
        enemyPathfinding.MoveInDirection(direction);
    }

    public void MoveTowardPlayer()
    {
        if (DistanceToPlayer <= stats.stoppingDistance)
        {
            enemyPathfinding.StopMoving();
            return;
        }

        Vector2 directionToPlayer = (PlayerController.Instance.transform.position - transform.position).normalized;
        enemyPathfinding.MoveInDirection(directionToPlayer);
    }

    public void TryAttack()
    {
        if (!CanAttack || Time.time < nextAttackTime) { return; }

        nextAttackTime = Time.time + stats.attackCooldown;
        attack.Attack();
    }

    private void OnDrawGizmosSelected()
    {
        if (stats == null) { return; }

        DrawRange(stats.detectionRange, Color.yellow);
        DrawRange(stats.loseRange, Color.gray);
        DrawRange(stats.attackRange, Color.red);
        DrawRange(stats.stoppingDistance, Color.cyan);
    }

    private void DrawRange(float radius, Color color)
    {
        if (radius <= 0f) { return; }

        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
