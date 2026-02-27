using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BearController : MonoBehaviourPun, IPunObservable
{
    public BearStat Stat;

    [Header("Distance Settings")]
    [SerializeField] private float _patrolRange = 30f;
    [SerializeField] private float _detectRange = 10f;    // Wait 상태 진입 범위
    [SerializeField] private float _chaseRange = 7f;     // Chase 상태 진입 범위
    [SerializeField] private float _attackRange = 3f;    // Attack 상태 진입 범위

    [Header("Monster Status")]
    [SerializeField] private BearState _currentState = BearState.Idle;

    private BearCombat _combat;

    private Animator _bearAnimator;
    private static readonly int _stateHash = Animator.StringToHash("State");

    private NavMeshAgent _agent;
    private Transform _target;
    private Vector3 _patrolPos;

    private bool _isWaiting = false;

    public event Action OnHealthChanged;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 읽기(쓰기) 모드
        if (stream.IsWriting)
        {
            // 이 PhotonView의 데이터를 보내줘야하는 상황
            stream.SendNext(Stat.CurrentHealth);  // 현재 체력과 스태미나
            // 보낼 값이 많아진다면 JSON이나 BinaryFormatter로 직렬화해서 보내는 방법도 있다.
            // SendNext, ReceiveNext에 쓰이는 캐스팅에 필요한 박싱 언박싱의 비용과 JsonUtility의 비용을 비교해서 선택하면 된다.
        }

        else if (stream.IsReading)
        {
            // 이 PhotonView의 데이터를 받아야하는 상황
            // ReceiveNext()는 object 타입이므로, 캐스팅 필요
            // 받는 쪽에서는, 보내는 쪽에서 보낸 순서대로 ReceiveNext()를 호출해야한다.
            Stat.CurrentHealth = (float)stream.ReceiveNext();
            OnHealthChanged?.Invoke(); // ← 네트워크 동기화 시
        }
    }
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _bearAnimator = GetComponent<Animator>();
        _combat = GetComponent<BearCombat>();
        Stat.Initialize();
        AgentInitialize();
        SetNewPatrolDestination();
        _combat.Initialize(Stat);
    }

    private void Start()
    {
        _combat.OnHitTaken += pos =>
        {
            if (_currentState == BearState.Hit) return;
            HandleKnockback(pos);
            ChangeState(BearState.Hit);
        };
        _combat.OnDead += () => ChangeState(BearState.Death);
        _combat.OnHit += () => ChangeState(BearState.Hit);
        _combat.OnAttackFinished += () =>
        {
            if (_currentState == BearState.Attack)
            {
                ChangeState(BearState.Chase);
            }
        };
        _combat.OnHealthChanged += () => OnHealthChanged?.Invoke();
    }

    private void OnDestroy()
    {
        _combat.OnHitTaken -= pos =>
        {
            if (_currentState == BearState.Hit) return;
            HandleKnockback(pos);
            ChangeState(BearState.Hit);
        };
        _combat.OnDead -= () => ChangeState(BearState.Death);
        _combat.OnHit -= () => ChangeState(BearState.Hit);
        _combat.OnAttackFinished -= () =>
        {
            if (_currentState == BearState.Attack)
            {
                ChangeState(BearState.Chase);
            }
        };
        _combat.OnHealthChanged -= () => OnHealthChanged?.Invoke();
    }

    void Update()
    {
        // 마스터 클라이언트만 AI 로직을 계산함
        if (!PhotonNetwork.IsMasterClient) return;
        if (Stat.IsDead) return;
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
                else if (_agent.remainingDistance <= 0.1f && !_isWaiting)
                {
                    _isWaiting = true;
                    _agent.isStopped = true;
                    StartCoroutine(WaitAndPatrol());
                }
                break;

            case BearState.Patrol:
                _agent.speed = Stat.Speed;
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
                _agent.speed = Stat.ChaseSpeed;
                _agent.SetDestination(_target.position);
                if (distance <= _attackRange) ChangeState(BearState.Attack);
                else if (distance > _detectRange) ChangeState(BearState.Patrol);
                break;

            case BearState.Death:
                _agent.isStopped = true;
                break;
        }
    }

    public void ChangeState(BearState newState)
    {
        if (_currentState == newState) return;                                     // 동일 상태 재진입 방지
        if (_currentState == BearState.Hit && newState != BearState.Idle) return; // 피격 중엔 상태변경 제한
        StopAllCoroutines();
        _combat.StopAttack();
        _isWaiting = false;
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
            _combat.StartAttack(_target, _attackRange);
        }
        else if (newState == BearState.Death)
        {
            _agent.isStopped = true;
            GetComponent<Collider>().enabled = false; // 시체 공격 방지
            StartCoroutine(DestroyAfterDeath());
        }
    }


    IEnumerator WaitAndPatrol()
    {
        yield return new WaitForSeconds(Stat.PatrolWaitTime);
        _isWaiting = false;
        SetNewPatrolDestination();
        ChangeState(BearState.Patrol);
    }

    void SetNewPatrolDestination()
    {
        Vector3 randomDir = UnityEngine.Random.insideUnitSphere * _patrolRange;
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
    private void RPC_SyncAnimation(BearState state)
    {
        // Animator 파라미터 업데이트 (예: state 정수형으로 넘기기)
        _bearAnimator.SetInteger(_stateHash, (int)state);
    }

    private IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(5f);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject); // MasterClient에서만 호출됨
        }
    }

    private void AgentInitialize()
    {
        _agent.speed = Stat.Speed;
        _agent.acceleration = Stat.Acceleration;
        _agent.angularSpeed = Stat.AngularSpeed;
    }

    private void HandleKnockback(Vector3 attackerPos)
    {
        Vector3 knockbackDir = transform.position - attackerPos;
        knockbackDir.y = 0;
        knockbackDir.Normalize();
        _agent.updateRotation = false;
        _agent.velocity = knockbackDir * Stat.KnockbackForce;
        Invoke("ResetAfterHit", Stat.KnockbackTime);
    }
    private void ResetAfterHit()
    {
        _agent.updateRotation = true;            // 회전 복구
        ChangeState(BearState.Idle);
    }
}
