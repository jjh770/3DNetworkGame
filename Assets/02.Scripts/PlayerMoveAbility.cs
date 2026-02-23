using UnityEngine;

public class PlayerMoveAbility : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _jumpForce = 2.5f;
    private const float GRAVITY = 9.8f;
    [SerializeField] private float _yVelocity = 0f;
    [SerializeField] private Animator _animator;

    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        Vector3 direction = new Vector3(h, 0, v);
        direction.Normalize();

        if (_animator != null)
        {
            _animator.SetFloat("MoveX", direction.x);
            _animator.SetFloat("MoveZ", direction.z);
        }

        Vector3 moveDirection = transform.TransformDirection(direction);

        _yVelocity -= GRAVITY * Time.deltaTime;
        moveDirection.y = _yVelocity;

        if (Input.GetKey(KeyCode.Space) && _characterController.isGrounded)
        {
            _yVelocity = _jumpForce;
        }

        _characterController.Move(moveDirection * Time.deltaTime * _moveSpeed);
    }
}
