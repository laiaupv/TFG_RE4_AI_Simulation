using UnityEngine;

public class EnemyClassic : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Detección")]
    public float detectionRadius = 5f;

    private StateMachine _stateMachine;
    private Transform _player;
    private LineRenderer _lineRenderer;
    private int _circleSegments = 40;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _stateMachine = new StateMachine();
        _stateMachine.ChangeState(new IdleState(
            transform, waypoints, _stateMachine, _player));

        _lineRenderer = GetComponent<LineRenderer>();
        DrawDetectionCircle();
    }

    void Update()
    {
        _stateMachine.Update();
        UpdateCircleColor();
    }

    void DrawDetectionCircle()
    {
        _lineRenderer.positionCount = _circleSegments + 1;

        for (int i = 0; i <= _circleSegments; i++)
        {
            float angle = i * 2 * Mathf.PI / _circleSegments;
            float x = Mathf.Cos(angle) * detectionRadius;
            float z = Mathf.Sin(angle) * detectionRadius;
            _lineRenderer.SetPosition(i, new Vector3(x, 0.1f, z));
        }
    }

    void UpdateCircleColor()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, 
                                                   _player.position);
        if (distanceToPlayer < detectionRadius)
        {
            // Rojo cuando detecta al jugador
            _lineRenderer.startColor = Color.red;
            _lineRenderer.endColor = Color.red;
        }
        else
        {
            // Amarillo cuando no detecta
            _lineRenderer.startColor = Color.yellow;
            _lineRenderer.endColor = Color.yellow;
        }
    }

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