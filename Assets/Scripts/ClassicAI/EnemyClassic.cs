using UnityEngine;

public class EnemyClassic : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Detección")]
    public float detectionRadius = 5f;

    private StateMachine _stateMachine;
    private Transform _player;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _stateMachine = new StateMachine();
        _stateMachine.ChangeState(new IdleState(
            transform, waypoints, _stateMachine, _player));
    }

    void Update()
    {
        _stateMachine.Update();
    }

    // Dibuja el radio de detección en el editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    public string GetCurrentState()
{
    return _stateMachine.GetCurrentStateName();
}
}