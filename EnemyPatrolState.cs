using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyPatrolState : IState
{
    private const string UnitIsWalking = "isWalking";
    
    private readonly BasicEnemy _enemy;
    private readonly NavMeshAgent _navMeshAgent;
    private readonly Animator _animator;
    
    private Transform[] _patrolPoints;
    private int _currentPatrolIndex;
    
    private float _waitTime;
    private const float MinWaitTime = 5;
    private const float MaxWaitTime = 20;
    
    private bool _isWaiting;
    
    private static readonly HashSet<Transform> OccupiedPatrolPoints = new HashSet<Transform>();

    public EnemyPatrolState(BasicEnemy enemy, NavMeshAgent navMeshAgent, Animator animator)
    {
        _enemy = enemy;
        _navMeshAgent = navMeshAgent;
        _animator = animator;
    }

    public void Tick()
    {
        if (_isWaiting)
        {
            _waitTime -= Time.deltaTime;
            if (!(_waitTime <= 0)) return;
            _isWaiting = false;
            MoveToNextPatrolPoint();
            return;
        }

        if (!(_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)) return;
        _isWaiting = true;
        _waitTime = Random.Range(MinWaitTime, MaxWaitTime); // Random wait time at patrol points
        lock (OccupiedPatrolPoints)
        {
            OccupiedPatrolPoints.Remove(_patrolPoints[_currentPatrolIndex]);
        }
    }

    public void OnEnter()
    {
        Debug.Log("Enemy Patrol State");
        _animator.SetBool(UnitIsWalking, true);
        _patrolPoints = GetPatrolPoints();
        _currentPatrolIndex = Random.Range(0, _patrolPoints.Length); // Start at a random patrol point
        MoveToNextPatrolPoint();
    }

    public void OnExit()
    {
        _animator.SetBool(UnitIsWalking, false);
        lock (OccupiedPatrolPoints)
        {
            OccupiedPatrolPoints.Remove(_patrolPoints[_currentPatrolIndex]);
        }
    }

    private void MoveToNextPatrolPoint()
    {
        if (_patrolPoints.Length == 0) return;

        do
        {
            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
        } while (OccupiedPatrolPoints.Contains(_patrolPoints[_currentPatrolIndex]));

        lock (OccupiedPatrolPoints)
        {
            OccupiedPatrolPoints.Add(_patrolPoints[_currentPatrolIndex]);
        }
        _navMeshAgent.SetDestination(_patrolPoints[_currentPatrolIndex].position);
    }

    private Transform[] GetPatrolPoints()
    {
        var enemyZone = FindEnemyZone();
        return enemyZone != null ? enemyZone.PatrolPoints : GetDefaultPatrolPoints();
    }

    private EnemyZone FindEnemyZone()
    {
        var colliders = Physics.OverlapSphere(_enemy.transform.position, BasicEnemy.DetectionRange);
        foreach (var collider in colliders)
        {
            var enemyZone = collider.GetComponent<EnemyZone>();
            if (enemyZone != null && enemyZone.PatrolPoints.Length > 0)
            {
                return enemyZone;
            }
        }
        return null;
    }

    private Transform[] GetDefaultPatrolPoints()
    {
        var points = new Transform[4];
        for (var i = 0; i < points.Length; i++)
        {
            var point = new GameObject($"PatrolPoint_{i}").transform;
            point.position = _enemy.transform.position + Random.insideUnitSphere * 10f;
            points[i] = point;
        }
        return points;
    }
}