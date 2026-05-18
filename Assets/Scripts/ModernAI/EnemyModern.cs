using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyModern : MonoBehaviour
{
    [Header("Detección Visual")]
    public float visionRange = 8f;
    public float visionAngle = 60f;

    [Header("Detección Auditiva")]
    public float hearingRange = 10f;

    [Header("Ataque")]
    public float attackRange = 1.5f;

    [Header("Patrulla")]
    public Transform[] waypoints;

    private int _currentWaypoint = 0;
    private NavMeshAgent _agent;
    private Transform _player;
    private PlayerController _playerController;
    private Node _behaviorTree;
    private string _currentState = "Patrulla";
    private string _previousState = "";

    private LineRenderer _visionConeLR;
    private LineRenderer _hearingLineRenderer;
    private int _circleSegments = 40;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        if (_player == null)
        {
            Debug.LogError("No se encontró el Player.");
            return;
        }

        _playerController = _player.GetComponent<PlayerController>();

        if (_agent == null)
        {
            Debug.LogError("No se encontró el NavMesh Agent.");
            return;
        }

        BuildBehaviorTree();
        SetupVisuals();

        if (waypoints.Length > 0)
            _agent.SetDestination(waypoints[0].position);
    }

    void BuildBehaviorTree()
    {
        // Nodo de combate — detecta, persigue y ataca
        ActionNode combatNode = new ActionNode(() =>
        {
            if (!CanSeePlayer() && !CanHearPlayer())
                return NodeState.Failure;

            float distance = Vector3.Distance(transform.position, _player.position);

            // Si está en rango de ataque
            if (distance <= attackRange)
            {
                _agent.SetDestination(transform.position);
                _currentState = "Ataque";
                Debug.Log("Atacando!");
                return NodeState.Running;
            }

            // Si detecta al jugador pero no está en rango persigue
            _agent.SetDestination(_player.position);
            _currentState = "Persecución";

            Vector3 direction = _player.position - transform.position;
            direction.y = 0;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);

            return NodeState.Running;
        });

        // Nodo de alerta
        ActionNode alertNode = new ActionNode(() =>
        {
            if (CanSeePlayer() || CanHearPlayer())
            {
                _currentState = "Alerta";
                return NodeState.Success;
            }
            return NodeState.Failure;
        });

        // Nodo de patrulla con NavMesh
        ActionNode patrolNode = new ActionNode(() =>
        {
            if (_previousState != "Patrulla")
            {
                _currentWaypoint = GetNearestWaypoint();
                _agent.SetDestination(waypoints[_currentWaypoint].position);
            }

            _currentState = "Patrulla";

            if (waypoints.Length == 0) return NodeState.Running;

            if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
            {
                _currentWaypoint = (_currentWaypoint + 1) % waypoints.Length;
                _agent.SetDestination(waypoints[_currentWaypoint].position);
            }

            return NodeState.Running;
        });

        // Secuencia alerta → combate
        SequenceNode combatSequence = new SequenceNode(new List<Node>
        {
            alertNode,
            combatNode
        });

        // Selector raíz: intenta combate, si falla patrulla
        _behaviorTree = new SelectorNode(new List<Node>
        {
            combatSequence,
            patrolNode
        });
    }

    int GetNearestWaypoint()
    {
        int nearest = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < waypoints.Length; i++)
        {
            float distance = Vector3.Distance(transform.position,
                                              waypoints[i].position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = i;
            }
        }
        return nearest;
    }

    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = _player.position - transform.position;
        float distance = directionToPlayer.magnitude;

        if (distance > visionRange) return false;

        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > visionAngle) return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f,
                            directionToPlayer.normalized,
                            out hit, visionRange))
        {
            if (hit.transform.CompareTag("Player"))
                return true;
        }
        return false;
    }

    bool CanHearPlayer()
    {
        if (_playerController == null) return false;
        float distance = Vector3.Distance(transform.position, _player.position);
        float hearingStrength = 1 - (distance / hearingRange);
        return _playerController.isMakingNoise && hearingStrength > 0;
    }

    void SetupVisuals()
    {
        DrawVisionCone();

        GameObject hearingObj = new GameObject("HearingRadius");
        hearingObj.transform.parent = transform;
        hearingObj.transform.localPosition = Vector3.zero;
        _hearingLineRenderer = hearingObj.AddComponent<LineRenderer>();
        SetupLineRenderer(_hearingLineRenderer, Color.blue);
        DrawCircle(_hearingLineRenderer, hearingRange);
    }

    void DrawVisionCone()
    {
        int coneSegments = 20;
        GameObject coneObj = new GameObject("VisionCone");
        coneObj.transform.parent = transform;
        coneObj.transform.localPosition = Vector3.zero;
        _visionConeLR = coneObj.AddComponent<LineRenderer>();
        SetupLineRenderer(_visionConeLR, Color.green);
        _visionConeLR.positionCount = coneSegments + 3;
        _visionConeLR.useWorldSpace = false;

        _visionConeLR.SetPosition(0, Vector3.zero);

        for (int i = 0; i <= coneSegments; i++)
        {
            float angle = -visionAngle + (2 * visionAngle / coneSegments) * i;
            float rad = angle * Mathf.Deg2Rad;
            float x = Mathf.Sin(rad) * visionRange;
            float z = Mathf.Cos(rad) * visionRange;
            _visionConeLR.SetPosition(i + 1, new Vector3(x, 0.1f, z));
        }

        _visionConeLR.SetPosition(coneSegments + 2, Vector3.zero);
    }

    void SetupLineRenderer(LineRenderer lr, Color color)
    {
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.loop = false;
        lr.useWorldSpace = false;
        lr.startColor = color;
        lr.endColor = color;
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

    void Update()
    {
        if (_behaviorTree == null || _visionConeLR == null) return;
        _previousState = _currentState;
        _behaviorTree.Evaluate();
        UpdateVisionColor();
    }

    void UpdateVisionColor()
    {
        if (CanSeePlayer())
        {
            _visionConeLR.startColor = Color.red;
            _visionConeLR.endColor = Color.red;
        }
        else
        {
            _visionConeLR.startColor = Color.green;
            _visionConeLR.endColor = Color.green;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
    }

    public string GetCurrentState()
    {
        return _currentState;
    }
}