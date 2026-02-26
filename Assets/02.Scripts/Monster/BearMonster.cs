using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BearMonster : MonoBehaviourPun
{
    [Header("Distance Settings")]
    [SerializeField] private float _patrolRange = 10f;
    [SerializeField] private float _detectRange = 7f;    // Wait 상태 진입 범위
    [SerializeField] private float _chaseRange = 5f;     // Chase 상태 진입 범위
    [SerializeField] private float _attackRange = 2f;    // Attack 상태 진입 범위

    [Header("Monster Status")]
    [SerializeField] private BearState _currentState = BearState.Idle;
    private NavMeshAgent _agent;
    private Transform _target;
    private Vector3 _patrolPos;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        SetNewPatrolDestination();
    }

    void Update()
    {
        // 마스터 클라이언트만 AI 로직을 계산함
        if (!PhotonNetwork.IsMasterClient) return;

        UpdateAI();
    }

    private void UpdateAI()
    {
        FindNearestPlayer();

        float distance = _target != null ? Vector3.Distance(transform.position, _target.position) : float.MaxValue;

        switch (_currentState)
        {
            case BearState.Idle:
                if (distance <= _detectRange) ChangeState(BearState.Wait);
                else if (_agent.remainingDistance <= 0.1f) StartCoroutine(WaitAndPatrol());
                break;

            case BearState.Patrol:
                if (distance <= _detectRange) ChangeState(BearState.Wait);
                else if (_agent.remainingDistance <= 0.1f) ChangeState(BearState.Idle);
                break;

            case BearState.Wait:
                _agent.isStopped = true;
                if (distance <= _chaseRange) ChangeState(BearState.Chase);
                else if (distance > _detectRange) ChangeState(BearState.Patrol);
                break;

            case BearState.Chase:
                _agent.isStopped = false;
                _agent.SetDestination(_target.position);
                if (distance <= _attackRange) ChangeState(BearState.Attack);
                else if (distance > _detectRange) ChangeState(BearState.Patrol);
                break;
        }
    }

    public void ChangeState(BearState newState)
    {
        if (_currentState == BearState.Hit && newState != BearState.Idle) return; // 피격 중엔 상태변경 제한

        _currentState = newState;

        // 상태 변경 시 애니메이션 동기화를 위해 RPC 호출
        photonView.RPC("RPC_SyncAnimation", RpcTarget.All, newState);

        // 상태별 즉시 실행 로직
        if (newState == BearState.Patrol)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_patrolPos);
        }
        else if (newState == BearState.Attack)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    // 피격 시 호출되는 함수 (외부 데미지 로직에서 호출)
    [PunRPC]
    public void OnHit(Vector3 attackerPos)
    {
        if (_currentState == BearState.Hit) return;

        StopAllCoroutines();
        ChangeState(BearState.Hit);

        // 넉백 로직
        Vector3 knockbackDir = (transform.position - attackerPos).normalized;
        _agent.velocity = knockbackDir * 5f;

        Invoke("ResetAfterHit", 0.5f); // 0.5초 후 다시 행동
    }

    private void ResetAfterHit() => ChangeState(BearState.Idle);

    IEnumerator AttackRoutine()
    {
        _agent.isStopped = true;
        yield return new WaitForSeconds(1f); // 1초 대기
        // 공격 판정 로직 (여기서 실제 데미지 처리)
        yield return new WaitForSeconds(1f); // 후딜레이
        ChangeState(BearState.Chase);
    }

    IEnumerator WaitAndPatrol()
    {
        yield return new WaitForSeconds(2f);
        SetNewPatrolDestination();
        ChangeState(BearState.Patrol);
    }

    void SetNewPatrolDestination()
    {
        Vector3 randomDir = Random.insideUnitSphere * _patrolRange;
        randomDir += transform.position;
        NavMesh.SamplePosition(randomDir, out NavMeshHit hit, _patrolRange, 1);
        _patrolPos = hit.position;
    }

    void FindNearestPlayer()
    {
        // 멀티플레이어 환경에서 가장 가까운 플레이어 찾기
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDist = float.MaxValue;
        foreach (GameObject p in players)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                _target = p.transform;
            }
        }
    }

    [PunRPC]
    void RPC_SyncAnimation(BearState state)
    {
        // Animator 파라미터 업데이트 (예: state 정수형으로 넘기기)
        // animator.SetInteger("State", (int)state);
    }
}
