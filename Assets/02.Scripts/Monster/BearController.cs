using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BearController : MonoBehaviourPun
{
    public BearStat Stat;

    [Header("Distance Settings")]
    [SerializeField] private float _patrolRange = 30f;
    [SerializeField] private float _detectRange = 10f;    // Wait 상태 진입 범위
    [SerializeField] private float _chaseRange = 7f;     // Chase 상태 진입 범위
    [SerializeField] private float _attackRange = 3f;    // Attack 상태 진입 범위

    [Header("Monster Status")]
    [SerializeField] private BearState _currentState = BearState.Idle;

    [SerializeField] private BearWeaponCollider _weaponCollider;

    private BearCombat _combat;

    private Animator _bearAnimator;
    private static readonly int _stateHash = Animator.StringToHash("State");

    private NavMeshAgent _agent;
    private Transform _target;
    private Vector3 _patrolPos;

    private bool _isWaiting = false;

    public event Action OnHealthChanged;

    private bool _isRotating = false;
    private const float _rotateThreshold = 1f; // 1도 이상 차이날 때만 회전


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
        _combat.OnHitTaken += OnHitTakenHandler;
        _combat.OnDead += OnDeadHandler;
        _combat.OnHit += OnHitHandler;
        _combat.OnAttackFinished += OnAttackFinishedHandler;
        _combat.OnHealthChanged += OnHealthChangedHandler;
    }

    private void OnDestroy()
    {
        _combat.OnHitTaken -= OnHitTakenHandler;
        _combat.OnDead -= OnDeadHandler;
        _combat.OnHit -= OnHitHandler;
        _combat.OnAttackFinished -= OnAttackFinishedHandler;
        _combat.OnHealthChanged -= OnHealthChangedHandler;
    }

    private void OnHitTakenHandler(Vector3 pos)
    {
        if (_currentState == BearState.Hit) return;
        HandleKnockback(pos);
        ChangeState(BearState.Hit);
    }

    private void OnDeadHandler() => ChangeState(BearState.Death);

    private void OnHitHandler() => ChangeState(BearState.Hit);

    private void OnAttackFinishedHandler()
    {
        if (_currentState == BearState.Attack)
            ChangeState(BearState.Chase);
    }

    private void OnHealthChangedHandler() => OnHealthChanged?.Invoke();

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

            case BearState.Attack:
                RotationToTarget();
                break;
            case BearState.Patrol:
                _agent.speed = Stat.Speed;
                if (distance <= _detectRange) ChangeState(BearState.Wait);
                else if (_agent.remainingDistance <= 0.1f) ChangeState(BearState.Idle);
                break;

            case BearState.Wait:
                _agent.isStopped = true;
                RotationToTarget();
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
        if (_currentState == BearState.Death) return; // Death는 탈출 불가
        if (_currentState == BearState.Hit && newState != BearState.Idle && newState != BearState.Death) return;
        StopAllCoroutines();
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
        else if (newState == BearState.Wait)
        {
            StartCoroutine(WaitToChase());
        }
        else if (newState == BearState.Death)
        {
            _agent.isStopped = true;
            GetComponent<Collider>().enabled = false; // 시체 공격 방지
            _weaponCollider.DisableCollider();
            StartCoroutine(DestroyAfterDeath());
        }
    }

    private IEnumerator WaitToChase()
    {
        yield return new WaitForSeconds(5f);
        ChangeState(BearState.Chase);
    }
    // 애니메이션 이벤트: 타격 시작 프레임
    public void OnAttackStart()
    {
        _weaponCollider.EnableCollider();
    }

    // 애니메이션 이벤트: 타격 끝 프레임
    public void OnAttackEnd()
    {
        _weaponCollider.DisableCollider();
        if (!PhotonNetwork.IsMasterClient) return;
        _combat.OnAttackEnd();
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
        _target = null; // 추가
        float minDist = float.MaxValue;
        foreach (PlayerController player in PlayerController.All)
        {
            if (player == null || player.Stat.IsDead) continue;

            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                _target = player.transform;
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
        if (Stat.IsDead) return;
        _agent.updateRotation = true;            // 회전 복구
        ChangeState(BearState.Idle);
    }

    private void RotationToTarget()
    {
        if (_target == null) return;
        Vector3 dir = (_target.position - transform.position).normalized;
        dir.y = 0f;
        Quaternion targetRot = Quaternion.LookRotation(dir);

        float angle = Quaternion.Angle(transform.rotation, targetRot);

        if (angle > _rotateThreshold)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, _agent.angularSpeed * Time.deltaTime);
            SetRotating(true);
        }
        else
        {
            SetRotating(false);
        }
    }

    private void SetRotating(bool isRotate)
    {
        if (_isRotating == isRotate) return; // 변화 없으면 RPC 호출 방지
        _isRotating = isRotate;
        photonView.RPC("RPC_SetRotating", RpcTarget.All, isRotate);
    }

    [PunRPC]
    private void RPC_SetRotating(bool isRotate)
    {
        _bearAnimator.SetBool("IsRotating", isRotate);
    }
}
