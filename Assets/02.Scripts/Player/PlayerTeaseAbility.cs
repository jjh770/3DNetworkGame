using UnityEngine;

public class PlayerTeaseAbility : PlayerAbility
{
    [SerializeField] private Animator _animator;

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _animator.SetTrigger("Tease1");
        }
    }
}
