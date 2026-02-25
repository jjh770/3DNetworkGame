using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineCamera _cinemachineCamera;
    private CinemachineBasicMultiChannelPerlin noise;

    private void Awake()
    {
        _cinemachineCamera = GetComponent<CinemachineCamera>();
        noise = _cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Start()
    {
        PlayerHitAbility.OnHitPlayer += ShakeCamera;
        // 시작할 때는 흔들림이 없도록 0으로 초기화
        noise.AmplitudeGain = 0;
    }

    private void OnDestroy()
    {
        PlayerHitAbility.OnHitPlayer -= ShakeCamera;
    }

    public void ShakeCamera(float intensity, float duration)
    {
        StartCoroutine(ShakeRoutine(intensity, duration));
    }

    private IEnumerator ShakeRoutine(float intensity, float duration)
    {
        noise.AmplitudeGain = intensity;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // 시간이 지날수록 서서히 0으로 줄어들게 함 (Lerp 활용)
            noise.AmplitudeGain = Mathf.Lerp(intensity, 0, elapsed / duration);
            yield return null;
        }

        noise.AmplitudeGain = 0;
    }
}
