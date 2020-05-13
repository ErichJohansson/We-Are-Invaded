using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    private CinemachineVirtualCamera cam;
    private CinemachineBasicMultiChannelPerlin noise;

    public static CameraShakeController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        cam = FindObjectOfType<CinemachineVirtualCamera>();
        noise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float length, float amplitude, float frequency) {
        if (noise == null)
            return;

        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = frequency;

        StartCoroutine(Shake(length));
    }

    private IEnumerator Shake(float length)
    {
        yield return new WaitForSecondsRealtime(length);
        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;
    }
}
