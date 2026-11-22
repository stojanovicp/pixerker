public class StateMachine
{
    public IState CurrentState { get; private set; }

    public void ChangeState(IState nextState)
    {
        CurrentState?.Exit();
        CurrentState = nextState;
        CurrentState.Enter();
    }

    public void Tick()
    {
        CurrentState?.Tick();
    }
}
