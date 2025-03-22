using UnityEngine;
using UnityEngine.AI;

public class EnemyIdleState : IState
{
   

    private readonly Animator _animator;
    
    private float _idleTime;
    private const float MaxIdleTime = 5f;

    public EnemyIdleState( Animator animator)
    {
        _animator = animator;
    }

    public void Tick()
    {
        _idleTime += Time.deltaTime;
        if (_idleTime >= MaxIdleTime)
        {
            // Transition to patrol state or other logic
        }
    }

    public void OnEnter()
    {
        _idleTime = 0f;
        Debug.Log("Enemy Idle State");
        
    }

    public void OnExit()
    {
        // Any cleanup logic if needed
    }
}