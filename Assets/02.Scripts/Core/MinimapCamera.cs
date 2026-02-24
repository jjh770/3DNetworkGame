using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    private Transform _target;
    [SerializeField] private float _offsetY = 30f;

    private void OnEnable()
    {
        PlayerController.OnLocalPlayerSpawned += SetTarget;
    }

    private void OnDisable()
    {
        PlayerController.OnLocalPlayerSpawned -= SetTarget;
    }

    private void SetTarget(Transform target)
    {
        _target = target;
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        Vector3 targetPosition = _target.position;
        Vector3 finalPosition = targetPosition + new Vector3(0f, _offsetY, 0f);
        transform.position = finalPosition;

        Vector3 targetAngle = _target.eulerAngles;
        targetAngle.x = 90;
        transform.eulerAngles = targetAngle;
    }
}
