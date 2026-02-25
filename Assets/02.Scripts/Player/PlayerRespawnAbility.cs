using Photon.Pun;
using UnityEngine;

public class PlayerRespawnAbility : PlayerAbility
{
    [SerializeField] private Animator _animator;
    protected override void OnUpdate()
    {

    }

    public void Respawn(Transform respawnTransform)
    {
        CharacterController characterController = GetComponent<CharacterController>();
        characterController.enabled = false;
        transform.position = respawnTransform.position;
        transform.rotation = respawnTransform.rotation;
        characterController.enabled = true;
        PlayRespawnAnimation();
        _owner.PhotonView.RPC(nameof(PlayRespawnAnimation), Photon.Pun.RpcTarget.Others);
        _owner.Stat.Initialize();
        _owner.NotifySpawned();
    }

    [PunRPC]
    private void PlayRespawnAnimation()
    {
        _animator.SetTrigger("Respawn");
    }
}
