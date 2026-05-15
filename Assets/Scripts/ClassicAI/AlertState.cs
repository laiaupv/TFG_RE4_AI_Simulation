using UnityEngine;

public class AlertState : IState
{
    private Transform _enemy;
    private StateMachine _stateMachine;
    private Transform _player;
    private Transform[] _waypoints;
    private float _alertTime = 2f;
    private float _timer = 0f;
    private float _chaseRadius = 4f;

    public AlertState(Transform enemy, StateMachine stateMachine,
                      Transform player, Transform[] waypoints)
    {
        _enemy = enemy;
        _stateMachine = stateMachine;
        _player = player;
        _waypoints = waypoints;
    }

    public void OnEnter()
    {
        Debug.Log("Estado: Alerta");
        _timer = 0f;
    }

    public void OnUpdate()
    {
        _timer += Time.deltaTime;

        // Mira hacia el jugador
        Vector3 direction = _player.position - _enemy.position;
        direction.y = 0;
        if (direction != Vector3.zero)
            _enemy.rotation = Quaternion.LookRotation(direction);

        float distanceToPlayer = Vector3.Distance(_enemy.position, _player.position);

        // Si el jugador está muy cerca pasa a perseguir
        if (distanceToPlayer < _chaseRadius)
        {
            _stateMachine.ChangeState(new ChaseState(_enemy, _stateMachine,
                                                     _player, _waypoints));
            return;
        }

        // Si pasa el tiempo de alerta vuelve a patrullar
        if (_timer >= _alertTime)
        {
            _stateMachine.ChangeState(new IdleState(_enemy, _waypoints,
                                                    _stateMachine, _player));
        }
    }

    public void OnExit()
    {
        Debug.Log("Saliendo de Alerta");
    }
}