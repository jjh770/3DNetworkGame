using UnityEngine;

public class PlayerWeaponColliderAbility : PlayerAbility
{
    [SerializeField] private Collider _collider;

    private void Start()
    {
        DeactiveCollider();
    }

    protected override void OnUpdate()
    {

    }

    public void ActiveCollider()
    {
        _collider.enabled = true;
    }

    public void DeactiveCollider()
    {
        _collider.enabled = false;
    }
}
