using UnityEngine;

public abstract class PlayerAbility : MonoBehaviour
{
    protected PlayerController _owner { get; private set; }

    protected virtual void Awake()
    {
        _owner = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        if (!_owner.PhotonView.IsMine) return;
        if (_owner.Stat.IsDead) return;
        OnUpdate();
    }

    protected abstract void OnUpdate();
}
