using UnityEngine;

public class PlayerMoveAbility : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _jumpForce = 2.5f;
    private const float GRAVITY = 9.8f;
    [SerializeField] private float _yVelocity = 0f;

    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(v, 0, h);
        direction.Normalize();

        _yVelocity -= GRAVITY * Time.deltaTime;
        direction.y = _yVelocity;

        if (Input.GetKey(KeyCode.Space) && _characterController.isGrounded)
        {
            _yVelocity = _jumpForce;
        }

        _characterController.Move(direction * Time.deltaTime * _moveSpeed);
    }
}
