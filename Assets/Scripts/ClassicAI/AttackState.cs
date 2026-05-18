using UnityEngine;

public class AttackState : IState
{
    private Transform _enemy;
    private StateMachine _stateMachine;
    private Transform _player;
    private Transform[] _waypoints;
    private float _attackCooldown = 1f;
    private float _timer = 0f;
    private float _chaseRadius = 2f;
    private PlayerHealth _playerHealth;

    public AttackState(Transform enemy, StateMachine stateMachine,
                       Transform player, Transform[] waypoints)
    {
        _enemy = enemy;
        _stateMachine = stateMachine;
        _player = player;
        _waypoints = waypoints;
        _playerHealth = player.GetComponent<PlayerHealth>();
    }

    public void OnEnter()
    {
        Debug.Log("Estado: Ataque");
        _timer = 0f;
    }

    public void OnUpdate()
    {
        _timer += Time.deltaTime;

        Vector3 direction = _player.position - _enemy.position;
        direction.y = 0;
        if (direction != Vector3.zero)
            _enemy.rotation = Quaternion.LookRotation(direction);

        float distanceToPlayer = Vector3.Distance(_enemy.position, _player.position);

        if (distanceToPlayer > _chaseRadius)
        {
            _stateMachine.ChangeState(new ChaseState(_enemy, _stateMachine,
                                                     _player, _waypoints));
            return;
        }

        if (_timer >= _attackCooldown)
        {
            _timer = 0f;
            if (_playerHealth != null)
                _playerHealth.TakeDamage(20f);
            Debug.Log("¡Atacando al jugador!");
        }
    }

    public void OnExit()
    {
        Debug.Log("Saliendo de Ataque");
    }
}