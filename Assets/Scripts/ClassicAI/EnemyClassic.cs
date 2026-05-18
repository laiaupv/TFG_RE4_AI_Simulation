using UnityEngine;

public class EnemyClassic : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Detección Visual")]
    public float detectionRadius = 5f;

    [Header("Detección Auditiva")]
    public float hearingRadius = 8f;

    private StateMachine _stateMachine;
    private Transform _player;
    private PlayerController _playerController;
    private LineRenderer _lineRenderer;
    private LineRenderer _hearingLineRenderer;
    private int _circleSegments = 40;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerController = _player.GetComponent<PlayerController>();
        _stateMachine = new StateMachine();
        _stateMachine.ChangeState(new IdleState(
            transform, waypoints, _stateMachine, _player));

        // Radio visual
        _lineRenderer = GetComponent<LineRenderer>();
        DrawCircle(_lineRenderer, detectionRadius);

        // Radio auditivo
        GameObject hearingObj = new GameObject("HearingRadius");
        hearingObj.transform.parent = transform;
        hearingObj.transform.localPosition = Vector3.zero;
        _hearingLineRenderer = hearingObj.AddComponent<LineRenderer>();
        _hearingLineRenderer.material = _lineRenderer.material;
        _hearingLineRenderer.startWidth = 0.05f;
        _hearingLineRenderer.endWidth = 0.05f;
        _hearingLineRenderer.loop = true;
        _hearingLineRenderer.useWorldSpace = false;
        _hearingLineRenderer.startColor = Color.blue;
        _hearingLineRenderer.endColor = Color.blue;
        DrawCircle(_hearingLineRenderer, hearingRadius);
    }

    void Update()
    {
        _stateMachine.Update();
        UpdateVisualCircleColor();
        CheckHearing();
    }

    void DrawCircle(LineRenderer lr, float radius)
    {
        lr.positionCount = _circleSegments + 1;
        for (int i = 0; i <= _circleSegments; i++)
        {
            float angle = i * 2 * Mathf.PI / _circleSegments;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            lr.SetPosition(i, new Vector3(x, 0.1f, z));
        }
    }

    void UpdateVisualCircleColor()
    {
        float distanceToPlayer = Vector3.Distance(transform.position,
                                                   _player.position);
        if (distanceToPlayer < detectionRadius)
        {
            _lineRenderer.startColor = Color.red;
            _lineRenderer.endColor = Color.red;
        }
        else
        {
            _lineRenderer.startColor = Color.yellow;
            _lineRenderer.endColor = Color.yellow;
        }
    }

    void CheckHearing()
    {
        if (_playerController == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position,
                                                   _player.position);

        // Si el jugador hace ruido dentro del radio auditivo
        if (_playerController.isMakingNoise && distanceToPlayer < hearingRadius)
        {
            string currentState = _stateMachine.GetCurrentStateName();
            if (currentState == "IdleState")
            {
                _stateMachine.ChangeState(new AlertState(
                    transform, _stateMachine, _player, waypoints));
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }

    public string GetCurrentState()
    {
        return _stateMachine.GetCurrentStateName();
    }
}