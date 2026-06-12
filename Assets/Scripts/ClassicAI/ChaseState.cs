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
        Vector3 targetPos = new Vector3(_player.position.x,
                                        _enemy.position.y,
                                        _player.position.z);
        Vector3 moveDir = (targetPos - _enemy.position).normalized;
        _characterController.Move(moveDir * _speed * Time.deltaTime);

        Vector3 direction = _player.position - _enemy.position;
        direction.y = 0;
        if (direction != Vector3.zero)
            _enemy.rotation = Quaternion.LookRotation(direction);

        float distanceToPlayer = Vector3.Distance(_enemy.position, _player.position);

        if (distanceToPlayer < _attackRadius)
        {
            //Debug.Log("[FSM] TRANSICIÓN ATTACK - Distancia: " + distanceToPlayer.ToString("F2") + "u");
            _stateMachine.ChangeState(new AttackState(_enemy, _stateMachine,
                                                      _player, _waypoints));
            return;
        }

        if (distanceToPlayer > _loseRadius)
        {
            //Debug.Log("[FSM] PIERDE AL JUGADOR - Distancia: " + distanceToPlayer.ToString("F2") + "u");
            _stateMachine.ChangeState(new AlertState(_enemy, _stateMachine,
                                                     _player, _waypoints));
        }
    }

    public void OnExit()
    {
        Debug.Log("Saliendo de Persecución");
    }
}