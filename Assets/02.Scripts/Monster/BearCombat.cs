using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.AI;

public class BearCombat : MonoBehaviour, IDamageable
{
    private BearStat _stat;
    public BearStat Stat => _stat;
    private NavMeshAgent _agent;

    private Transform _target;
    private float _attackRange;

    public event Action OnDead;
    public event Action<Vector3> OnHitTaken;
    public event Action OnAttackFinished;
    public event Action OnHit;
    public event Action OnHealthChanged;

    public void Initialize(BearStat stat)
    {
        _stat = stat;
        _agent = GetComponent<NavMeshAgent>();
    }

    public void StartAttack(Transform target, float attackRange)
    {
        _target = target;
        _attackRange = attackRange;
        _agent.isStopped = true;
    }

    public void OnAttackEnd()
    {
        float dist = _target != null
            ? Vector3.Distance(transform.position, _target.position)
            : float.MaxValue;

        if (dist > _attackRange)
            OnAttackFinished?.Invoke();
        // 범위 안이면 애니메이션 루프 → 자연스럽게 다음 공격
    }

    [PunRPC]
    public void TakeDamage(float damage, int attackerActorNumber, AttackerType attackerType)
    {
        if (_stat.IsDead) return;
        _stat.TakeDamage(damage);
        OnHealthChanged?.Invoke();
        if (!PhotonNetwork.IsMasterClient) return;
        if (_stat.IsDead)
        {
            OnDead?.Invoke();
            return;
        }
        Vector3 attackerPos = transform.position;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.TryGetComponent(out PhotonView photonView) && photonView.Owner.ActorNumber == attackerActorNumber)
            {
                attackerPos = player.transform.position;
                break;
            }
        }
        OnHitTaken?.Invoke(attackerPos);
        OnHit?.Invoke();
    }
}
