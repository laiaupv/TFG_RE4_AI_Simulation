using UnityEngine;
using UnityEngine.AI;

public class EnemyModernAnimator : MonoBehaviour
{
    private Animator _animator;
    private EnemyModern _enemy;
    private NavMeshAgent _agent;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _enemy = GetComponent<EnemyModern>();
        _agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (_animator == null || _enemy == null) return;

        string state = _enemy.GetCurrentState();
        float speed = _agent.velocity.magnitude;

        switch (state)
        {
            case "Patrulla":
                _animator.SetFloat("Speed", speed > 0.1f ? 0.3f : 0f);
                _animator.SetBool("IsAttacking", false);
                break;
            case "Alerta":
                _animator.SetFloat("Speed", 0f);
                _animator.SetBool("IsAttacking", false);
                break;
            case "Persecución":
                _animator.SetFloat("Speed", 1f);
                _animator.SetBool("IsAttacking", false);
                break;
            case "Ataque":
                _animator.SetFloat("Speed", 0f);
                _animator.SetBool("IsAttacking", true);
                break;
        }
    }
}