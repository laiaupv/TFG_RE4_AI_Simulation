using UnityEngine;

public class EnemyClassicAnimator : MonoBehaviour
{
    private Animator _animator;
    private EnemyClassic _enemy;
    private Vector3 _lastPosition;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _enemy = GetComponent<EnemyClassic>();
        _lastPosition = transform.position;
    }

    void Update()
    {
        if (_animator == null || _enemy == null) return;

        string state = _enemy.GetCurrentState();

        // Calcula velocidad real
        float speed = Vector3.Distance(transform.position, _lastPosition) 
                      / Time.deltaTime;
        _lastPosition = transform.position;

        switch (state)
        {
            case "IdleState":
                _animator.SetFloat("Speed", speed > 0.1f ? 0.3f : 0f);
                _animator.SetBool("IsAttacking", false);
                break;
            case "AlertState":
                _animator.SetFloat("Speed", 0f);
                _animator.SetBool("IsAttacking", false);
                break;
            case "ChaseState":
                _animator.SetFloat("Speed", 1f);
                _animator.SetBool("IsAttacking", false);
                break;
            case "AttackState":
                _animator.SetFloat("Speed", 0f);
                _animator.SetBool("IsAttacking", true);
                break;
        }
    }
}