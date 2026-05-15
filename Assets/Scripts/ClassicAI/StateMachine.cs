public class StateMachine
{
    private IState _currentState;

    public void ChangeState(IState newState)
    {
        if (_currentState != null)
            _currentState.OnExit();

        _currentState = newState;
        _currentState.OnEnter();
    }

    public void Update()
    {
        if (_currentState != null)
            _currentState.OnUpdate();
    }

    public string GetCurrentStateName()
    {
        if (_currentState == null) return "None";
        return _currentState.GetType().Name;
    }
}