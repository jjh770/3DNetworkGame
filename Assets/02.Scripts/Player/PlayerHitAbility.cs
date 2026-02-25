using Photon.Pun;
using System;
using UnityEngine;
public class PlayerHitAbility : PlayerAbility, IDamageable
{
    [SerializeField] private Animator _animator;
    public static event Action<float, float> OnHitPlayer;
    protected override void OnUpdate()
    {

    }

    [PunRPC] // TakeDamage는 내 방에서의 상대 플레이어가 아닌 상대 방의 상대 플레이어에게 줘야하므로 RPC로 호출한다.
    public void TakeDamage(float damage)
    {
        if (_owner.Stat.IsDead) return;

        Debug.Log($"{damage} 데미지 피격!");
        _owner.Stat.TakeDamage(damage);
        OnHitPlayer?.Invoke(3, 1);

        if (_owner.Stat.IsDead && _owner.PhotonView.IsMine)
        {
            Debug.Log($"{_owner.PhotonView.name}이(가) 사망했습니다.");
            PlayDieAnimation();
            _owner.PhotonView.RPC("PlayDieAnimation", RpcTarget.Others);
            SpawnManager.Instance.RequestRespawn(_owner);
        }
    }

    [PunRPC]
    private void PlayDieAnimation()
    {
        _animator.SetTrigger("Die");
    }
}
