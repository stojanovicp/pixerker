public class EnemyChaseState : IState
{
    private readonly EnemyAI enemy;

    public EnemyChaseState(EnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public void Enter() { }

    public void Tick()
    {
        float distanceToPlayer = enemy.DistanceToPlayer;

        if (distanceToPlayer > enemy.Stats.loseRange)
        {
            enemy.ChangeState(enemy.RoamState);
            return;
        }

        if (enemy.CanAttack && distanceToPlayer <= enemy.Stats.attackRange)
        {
            enemy.ChangeState(enemy.AttackState);
            return;
        }

        enemy.MoveTowardPlayer();
    }

    public void Exit() { }
}
