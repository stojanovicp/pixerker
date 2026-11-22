using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private EnemyStats stats;
    [SerializeField] private MonoBehaviour enemyType;

    public int ContactDamage => stats.contactDamage;

    private enum State {
        Roaming,
        Attacking
    }

    private bool canAttack = true;
    private Vector2 roamPosition;
    private float timeRoaming = 0f;
    private State state;
    private EnemyPathfinding enemyPathfinding;

    private void Awake() {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        state = State.Roaming;
    }

    private void Start() {
        roamPosition = GetRoamingPosition();
    }

    private void Update()
    {
        if (PlayerController.Instance == null) { return; }

        MovementStateControl();
    }

    private void MovementStateControl()
    {
        switch(state)
        {
            case State.Roaming:
                Roaming();
            break;

            case State.Attacking:
                Attacking();
            break;

            default:
            break;
        }
    }

    private void Roaming()
    {
        timeRoaming += Time.deltaTime;

        enemyPathfinding.MoveTo(roamPosition);

        if(Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < stats.attackRange)
        {
            state = State.Attacking;
        }

        if(timeRoaming > stats.roamDirectionChangeTime)
        {
            roamPosition = GetRoamingPosition();
        }
    }

    private void Attacking()
    {
        if(Vector2.Distance(transform.position, PlayerController.Instance.transform.position) > stats.attackRange)
        {
            state = State.Roaming;
        }

        if(stats.attackRange != 0 && canAttack)
        {
            canAttack = false;
            (enemyType as IEnemy).Attack();

            if(stats.stopMovingWhileAttacking)
            {
                enemyPathfinding.StopMoving();
            }
            else
            {
                enemyPathfinding.MoveTo(roamPosition);
            }

            StartCoroutine(AttackCooldownRoutine());
        }
    }

    private IEnumerator AttackCooldownRoutine()
    {
        yield return new WaitForSeconds(stats.attackCooldown);
        canAttack = true;
    }
    

    private Vector2 GetRoamingPosition() {
        timeRoaming = 0f;
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}

