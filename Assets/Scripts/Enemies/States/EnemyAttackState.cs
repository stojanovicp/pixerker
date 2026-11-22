public class EnemyAttackState : IState
{
    private readonly EnemyAI enemy;

    public EnemyAttackState(EnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public void Enter() { }

    public void Tick()
    {
        if (!enemy.IsPlayerAlive)
        {
            enemy.ChangeState(enemy.RoamState);
            return;
        }

        if (enemy.DistanceToPlayer > enemy.Stats.attackRange)
        {
            enemy.ChangeState(enemy.ChaseState);
            return;
        }

        enemy.MoveTowardPlayer();
        enemy.TryAttack();
    }

    public void Exit() { }
}
