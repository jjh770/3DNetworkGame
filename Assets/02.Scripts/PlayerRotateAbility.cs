using Unity.Cinemachine;
using UnityEngine;

public class PlayerRotateAbility : PlayerAbility
{
    [SerializeField] private Transform _cameraRoot;
    [SerializeField] private float _rotationSpeed = 100f;
    private float _mx;
    private float _my;

    private void Start()
    {
        if (!_owner.PhotonView.IsMine) return;

        Cursor.lockState = CursorLockMode.Locked;

        CinemachineCamera vcam = GameObject.Find("FollowCamera").GetComponent<CinemachineCamera>();
        vcam.Follow = _cameraRoot.transform;
    }

    protected override void OnUpdate()
    {
        _mx += Input.GetAxis("Mouse X") * _rotationSpeed * Time.deltaTime;
        _my += Input.GetAxis("Mouse Y") * _rotationSpeed * Time.deltaTime;

        _my = Mathf.Clamp(_my, -70f, 80f);

        transform.eulerAngles = new Vector3(0f, _mx, 0f);
        _cameraRoot.localRotation = Quaternion.Euler(-_my, 0f, 0f);
    }
}
