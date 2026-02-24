using UnityEngine;

public class PlayerMoveAbility : PlayerAbility
{
    [SerializeField] private Animator _animator;
    private const float GRAVITY = 9.8f;
    [SerializeField] private float _yVelocity = 0f;

    private static readonly int _moveXHash = Animator.StringToHash("MoveX");
    private static readonly int _moveZHash = Animator.StringToHash("MoveZ");

    private CharacterController _characterController;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    protected override void OnUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        Vector3 direction = new Vector3(h, 0, v);
        direction.Normalize();

        if (_animator != null)
        {
            _animator.SetFloat(_moveXHash, direction.x);
            _animator.SetFloat(_moveZHash, direction.z);
        }

        Vector3 moveDirection = transform.TransformDirection(direction);

        _yVelocity -= GRAVITY * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space) && _characterController.isGrounded)
        {
            if (_owner.Stat.TryConsumeStamina(_owner.Stat.JumpStamina))
            {
                _yVelocity = _owner.Stat.JumpPower;
            }
            else
            {
                Debug.Log("스태미나 부족 : 점프");
            }
        }

        moveDirection.y = _yVelocity;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && _owner.Stat.TryConsumeStamina(Time.deltaTime * _owner.Stat.RunStamina);

        if (isRunning)
        {
            moveDirection *= _owner.Stat.RunSpeed;
        }
        else
        {
            moveDirection *= _owner.Stat.MoveSpeed;
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                _owner.Stat.RecoverStamina(Time.deltaTime * _owner.Stat.GainStamina);
            }
        }

        _characterController.Move(moveDirection * Time.deltaTime);
    }
}
