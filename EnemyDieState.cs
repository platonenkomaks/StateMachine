using UnityEngine;
using UnityEngine.AI;

public class EnemyDieState : IState
{
    private readonly BasicEnemy _enemy;
    private readonly Animator _animator;
    private readonly NavMeshAgent _navMeshAgent;
    private static readonly int Die = Animator.StringToHash("Die");

    public EnemyDieState(BasicEnemy enemy, Animator animator, NavMeshAgent navMeshAgent)
    {
        _enemy = enemy;
        _animator = animator;
        _navMeshAgent = navMeshAgent;
    }

    public void Tick()
    {
        // Logic for dying state, if any
    }

    public void OnEnter()
    {
        _animator.SetTrigger(Die);
        _navMeshAgent.isStopped = true;
        // Additional logic for when the enemy dies, e.g., disable enemy components
    }

    public void OnExit()
    {
        
       
        // Any cleanup logic if needed
    }
}