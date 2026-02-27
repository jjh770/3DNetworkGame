using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BearCombat : MonoBehaviour, IDamageable
{
    private BearStat _stat;
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
        StartCoroutine(AttackRoutine());
    }
    public void StopAttack()
    {
        StopAllCoroutines();
        _target = null;
    }
    private IEnumerator AttackRoutine()
    {
        _agent.isStopped = true;
        while (true)
        {
            yield return new WaitForSeconds(_stat.AttackWaitTime);
            if (_target != null)
            {
                float distance = Vector3.Distance(transform.position, _target.position);
                if (distance <= _attackRange)
                {
                    PhotonView targetView = _target.GetComponent<PhotonView>();
                    targetView.RPC(nameof(PlayerHitAbility.TakeDamage), RpcTarget.All,
                        _stat.AttackDamage, PhotonNetwork.MasterClient.ActorNumber);
                }
            }
            yield return new WaitForSeconds(_stat.AttackDelayTime);

            float dist = _target != null
                ? Vector3.Distance(transform.position, _target.position)
                : float.MaxValue;

            if (dist > _attackRange)
            {
                OnAttackFinished?.Invoke();
                yield break;
            }
        }
    }

    [PunRPC]
    public void TakeDamage(float damage, int attackerActorNumber)
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
        OnHit?.Invoke();
        OnHitTaken?.Invoke(attackerPos);
    }
}
