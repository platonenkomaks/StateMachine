using UnityEngine;

public class EnemyAttackState : IState
{
    private static readonly int Attack = Animator.StringToHash("Attack");
    private readonly BasicEnemy _enemy;
    private readonly Animator _animator;
    private readonly PlayerStats _playerStats;

    private float _attackCooldown;
    private bool _isAttacking;

    private Transform TargetPlayer => _enemy.Player;
    public EnemyAttackState(BasicEnemy enemy, Animator animator, PlayerStats playerStats)
    {
        _enemy = enemy;
        _animator = animator;
        _playerStats = playerStats;
    }

    public void OnEnter()
    {
        ResetAttackState();
    }

    public void OnExit() { }

    public void Tick()
    {
        FaceToPlayer();
        if (_isAttacking) return;
        
        if (_attackCooldown > 0)
        {
            _attackCooldown -= Time.deltaTime;
            return;
        }

        if (PlayerInAttackRange() && PlayerInAttackCone())
        {
            StartAttack();
        }
    }

    private void FaceToPlayer()
    {
        if (TargetPlayer == null) return;

        var direction = (TargetPlayer.position - _enemy.transform.position).normalized;
        var lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        _enemy.transform.rotation = Quaternion.Slerp(_enemy.transform.rotation, lookRotation, Time.deltaTime * 20f);
    }
    public void ApplyDamageToPlayer()
    {
        if (_playerStats == null) return;

        var damage = _enemy.EnemyStats.damage.CalculateFinalValue();
        _playerStats.TakeDamage(damage);

        Debug.Log("Player attacked by enemy for " + damage + " damage");

        _isAttacking = false;
        _attackCooldown = _enemy.EnemyStats.attackCooldown;
    }

    public void SetIsAttackingFalse()
    {
        _isAttacking = false;
        _attackCooldown = _enemy.EnemyStats.attackCooldown;
    }

    private bool PlayerInAttackRange()
    {
        var distanceToPlayer = Vector3.Distance(_enemy.transform.position, _enemy.Player.position);
        return distanceToPlayer <= _enemy.EnemyStats.attackRange.CalculateFinalValue();
    }

    private bool PlayerInAttackCone()
    {
        var directionToPlayer = (_enemy.Player.position - _enemy.transform.position).normalized;
        var angle = Vector3.Angle(_enemy.transform.forward, directionToPlayer);
        return angle < 10f;
    }

    private void StartAttack()
    {
        _isAttacking = true;
        _animator.SetTrigger(Attack);
            //  MainSoundManager.PlayRandomSoundByType(SoundType.ENEMY3_ATTACK, 0.5f);
    }

    private void ResetAttackState()
    {
        _isAttacking = false;
    }
}