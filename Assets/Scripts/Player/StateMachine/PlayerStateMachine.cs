
public class PlayerStateMachine
{
    public PlayerStates currentState { get; private set; }
    public void Initialize(PlayerStates currentState)
    {
        this.currentState = currentState;
        currentState.Enter();
    }

    public void ChangeState(PlayerStates newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
