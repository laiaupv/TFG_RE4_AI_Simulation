using UnityEngine;

public class IdleState : IState
{
    private Transform _enemy;
    private Transform[] _waypoints;
    private int _currentWaypoint = 0;
    private float _speed = 2f;
    private float _waypointThreshold = 0.5f;
    private StateMachine _stateMachine;
    private Transform _player;
    private float _detectionRadius = 5f;
    private CharacterController _characterController;

    public IdleState(Transform enemy, Transform[] waypoints,
                     StateMachine stateMachine, Transform player)
    {
        _enemy = enemy;
        _waypoints = waypoints;
        _stateMachine = stateMachine;
        _player = player;
        _characterController = enemy.GetComponent<CharacterController>();
    }

    public void OnEnter()
    {
        Debug.Log("Estado: Patrulla");
    }

    public void OnUpdate()
    {
        if (_waypoints.Length == 0) return;

        Transform target = _waypoints[_currentWaypoint];

        // Movimiento con CharacterController
        Vector3 targetPos = new Vector3(target.position.x,
                                        _enemy.position.y,
                                        target.position.z);
        Vector3 moveDir = (targetPos - _enemy.position).normalized;
        _characterController.Move(moveDir * _speed * Time.deltaTime);

        // Mira hacia el waypoint
        Vector3 direction = target.position - _enemy.position;
        direction.y = 0;
        if (direction != Vector3.zero)
            _enemy.rotation = Quaternion.LookRotation(direction);

        // Comprueba si ha llegado al waypoint
        float distance = Vector3.Distance(
            new Vector3(_enemy.position.x, 0, _enemy.position.z),
            new Vector3(target.position.x, 0, target.position.z)
        );
        if (distance < _waypointThreshold)
            _currentWaypoint = (_currentWaypoint + 1) % _waypoints.Length;

        // Comprueba si detecta al jugador
        float distanceToPlayer = Vector3.Distance(_enemy.position, _player.position);
        if (distanceToPlayer < _detectionRadius)
            _stateMachine.ChangeState(new AlertState(_enemy, _stateMachine,
                                                     _player, _waypoints));
    }

    public void OnExit()
    {
        Debug.Log("Saliendo de Patrulla");
    }
}