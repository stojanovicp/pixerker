using UnityEngine;

public class EnemyRoamState : IState
{
    private readonly EnemyAI enemy;
    private float timeUntilDirectionChange;

    public EnemyRoamState(EnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        PickNewDirection();
    }

    public void Tick()
    {
        if (enemy.DistanceToPlayer < enemy.Stats.detectionRange)
        {
            enemy.ChangeState(enemy.ChaseState);
            return;
        }

        timeUntilDirectionChange -= Time.deltaTime;

        if (timeUntilDirectionChange <= 0f)
        {
            PickNewDirection();
        }
    }

    public void Exit() { }

    private void PickNewDirection()
    {
        Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        enemy.MoveInDirection(randomDirection);
        timeUntilDirectionChange = enemy.Stats.roamDirectionChangeTime;
    }
}
