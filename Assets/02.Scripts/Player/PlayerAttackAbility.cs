using Photon.Pun;
using UnityEngine;

public class PlayerAttackAbility : PlayerAbility
{
    private float _attackTimer = 0f;

    [SerializeField] private Animator _animator;
    [SerializeField] private float _comboResetTime = 1.0f;
    private int _comboStep = 0;
    private float _lastAttackTime = 0f;

    protected override void OnUpdate()
    {
        if (_animator == null) return;

        _attackTimer += Time.deltaTime;

        if (Time.time - _lastAttackTime > _comboResetTime)
        {
            _comboStep = 0;
        }

        if (Input.GetMouseButtonDown(0) && _attackTimer >= _owner.Stat.AttackCoolTime)
        {
            if (!_owner.Stat.TryConsumeStamina(_owner.Stat.AttackStamina))
            {
                Debug.Log("스태미나 부족 : 공격");
                return;
            }

            _attackTimer = 0f;

            PlayAttackAnimation(Random.Range(1, 4));
            _owner.PhotonView.RPC("PlayAttackAnimation", RpcTarget.Others, Random.Range(1, 4));
        }

        if (Input.GetMouseButtonDown(1) && _attackTimer >= _owner.Stat.AttackCoolTime)
        {
            if (!_owner.Stat.TryConsumeStamina(_owner.Stat.AttackStamina))
            {
                Debug.Log("스태미나 부족 : 공격");
                return;
            }

            _attackTimer = 0f;

            _lastAttackTime = Time.time;
            _comboStep++;

            if (_comboStep > 3)
            {
                _comboStep = 1;
            }

            // 1. 일반 메서드 호출 방식 (로컬에서만 애니메이션 재생) - 네트워크 상의 다른 클라이언트에서는 애니메이션이 재생되지 않음.
            PlayAttackAnimation(_comboStep);
            // 2. RPC 메서드 호출 방식: 네트워크 상의 다른 클라이언트에서도 동일한 애니메이션이 재생되도록 동기화
            _owner.PhotonView.RPC(nameof(PlayAttackAnimation), RpcTarget.Others, _comboStep);
        }
    }
    // 트랜스폼(위치, 회전, 스케일), 애니메이션(float파라미터)와 같이 상시로 동기화가 필요한 데이터는 IPunObservable 인터페이스를 구현하여 OnPhotonSerializeView() 메서드에서 동기화하는 방식이 일반적입니다.
    // 하지만 애니메이션 트리거처럼 간헐적으로 특정한 이벤트가 발생했을때만 변화하는 데이터를 동기화는 데이터 동기화가 아닌 이벤트 동기화 : RPC
    // RPC : Remote Procedure Call, 원격 프로시저 호출. 네트워크 상의 다른 클라이언트에서 특정 메서드를 호출하는 기능
    // RPC로 호출할 함수는 반드시 [PunRPC] 어트리뷰트를 붙여야 하며, PhotonView 컴포넌트를 통해 호출해야 합니다.
    [PunRPC]
    private void PlayAttackAnimation(int animationNumber)
    {
        _animator.SetTrigger($"Attack{animationNumber}");
    }
}
