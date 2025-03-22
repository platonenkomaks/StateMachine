using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : IState
{
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private readonly BasicEnemy _enemy;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly Animator _animator;
    private readonly Transform _targetPlayer;
    

    public EnemyChaseState(BasicEnemy enemy, NavMeshAgent navMeshAgent, Animator animator, Transform targetPlayer)
    {
        _enemy = enemy;
        _navMeshAgent = navMeshAgent;
        _animator = animator;
        _targetPlayer = targetPlayer;
    }

    public void Tick()
    {
        if (_targetPlayer == null) return;
        _navMeshAgent.SetDestination(_targetPlayer.position);
    }

    public void OnEnter()
    {
        Debug.Log("Enemy Chase State");
        _animator.SetBool(IsWalking, true);
        if (_targetPlayer != null)
        {
            _navMeshAgent.SetDestination(_targetPlayer.position);
        }
    }

    public void OnExit()
    {
        _animator.SetBool(IsWalking, false);
    }

  
}