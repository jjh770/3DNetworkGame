using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.AI;

public class BearCombat : MonoBehaviour, IDamageable, IPunObservable
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 읽기(쓰기) 모드
        if (stream.IsWriting)
        {
            // 이 PhotonView의 데이터를 보내줘야하는 상황
            stream.SendNext(_stat.CurrentHealth);  // 현재 체력과 스태미나
            // 보낼 값이 많아진다면 JSON이나 BinaryFormatter로 직렬화해서 보내는 방법도 있다.
            // SendNext, ReceiveNext에 쓰이는 캐스팅에 필요한 박싱 언박싱의 비용과 JsonUtility의 비용을 비교해서 선택하면 된다.
        }

        else if (stream.IsReading)
        {
            // 이 PhotonView의 데이터를 받아야하는 상황
            // ReceiveNext()는 object 타입이므로, 캐스팅 필요
            // 받는 쪽에서는, 보내는 쪽에서 보낸 순서대로 ReceiveNext()를 호출해야한다.
            _stat.CurrentHealth = (float)stream.ReceiveNext();
            OnHealthChanged?.Invoke(); // ← 네트워크 동기화 시
        }
    }

    public void Initialize(BearStat stat)
    {
        _stat = stat;
        _agent = GetComponent<NavMeshAgent>();
    }

    public float GetAttackDamage()
    {
        return _stat.AttackDamage;
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
