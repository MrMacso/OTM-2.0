using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [Header("Shake")]
    [SerializeField] private float defaultAmplitude = 2f;
    [SerializeField] private float defaultFrequency = 2f;
    [SerializeField] private float defaultDuration = 0.25f;

    private CinemachineBasicMultiChannelPerlin noise;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        if (cinemachineCamera == null)
        {
            cinemachineCamera = GetComponent<CinemachineCamera>();
        }

        if (cinemachineCamera != null)
        {
            noise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }

    public void Shake()
    {
        Shake(defaultAmplitude, defaultFrequency, defaultDuration);
    }

    public void Shake(float amplitude, float frequency, float duration)
    {
        if (noise == null)
        {
            Debug.LogWarning("No CinemachineBasicMultiChannelPerlin found on Cinemachine Camera.");
            return;
        }

        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
        }

        shakeRoutine = StartCoroutine(ShakeRoutine(amplitude, frequency, duration));
    }

    private IEnumerator ShakeRoutine(float amplitude, float frequency, float duration)
    {
        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;
        yield return new WaitForSeconds(duration);
        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;
        shakeRoutine = null;
    }
}
