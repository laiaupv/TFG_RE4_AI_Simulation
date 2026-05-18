using UnityEngine;

public class ChaseState : IState
{
    private Transform _enemy;
    private StateMachine _stateMachine;
    private Transform _player;
    private Transform[] _waypoints;
    private float _speed = 3.5f;
    private float _attackRadius = 1.5f;
    private float _loseRadius = 8f;
    private CharacterController _characterController;

    public ChaseState(Transform enemy, StateMachine stateMachine,
                      Transform player, Transform[] waypoints)
    {
        _enemy = enemy;
        _stateMachine = stateMachine;
        _player = player;
        _waypoints = waypoints;
        _characterController = enemy.GetComponent<CharacterController>();
    }

    public void OnEnter()
    {
        Debug.Log("Estado: Persecución");
    }

    public void OnUpdate()
    {
        // Movimiento con CharacterController
        Vector3 targetPos = new Vector3(_player.position.x,
                                        _enemy.position.y,
                                        _player.position.z);
        Vector3 moveDir = (targetPos - _enemy.position).normalized;
        _characterController.Move(moveDir * _speed * Time.deltaTime);

        // Mira hacia el jugador
        Vector3 direction = _player.position - _enemy.position;
        direction.y = 0;
        if (direction != Vector3.zero)
            _enemy.rotation = Quaternion.LookRotation(direction);

        float distanceToPlayer = Vector3.Distance(_enemy.position, _player.position);

        // Si está muy cerca pasa a atacar
        if (distanceToPlayer < _attackRadius)
        {
            _stateMachine.ChangeState(new AttackState(_enemy, _stateMachine,
                                                      _player, _waypoints));
            return;
        }

        // Si el jugador huye vuelve a alerta
        if (distanceToPlayer > _loseRadius)
        {
            _stateMachine.ChangeState(new AlertState(_enemy, _stateMachine,
                                                     _player, _waypoints));
        }
    }

    public void OnExit()
    {
        Debug.Log("Saliendo de Persecución");
    }
}