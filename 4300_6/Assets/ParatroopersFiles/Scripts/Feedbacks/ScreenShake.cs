using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// To be put on a Cinemachine VirtualCamera.

public class ScreenShake : MonoBehaviour
{
    // Inspector variables
    [SerializeField] [Range(0, 2)] float[] firingScreenShakeIntensities = new float[(int)Weapon.MINIGUN + 1]; // 0: pistol, 1: shotgun, 2: sniper, 3: bazooka, 4: minigun
    [SerializeField] float shakeMultiplier = 3f;

    // Private variables
    CinemachineVirtualCamera virtualCamera = null;
    float time;
    bool amplitudeNeedsResetting;

    // Public methods
    public void ShakeScreen(Weapon type)
    {
        time = firingScreenShakeIntensities[(int)type];
        amplitudeNeedsResetting = true;
    }

    // Inherited methods.
    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
    private void FixedUpdate()
    {
        if (time > 0)
        {
            virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = time * shakeMultiplier;
            virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = time * shakeMultiplier;
        }
        else
        {
            if (amplitudeNeedsResetting)
            {
                virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
                virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
                amplitudeNeedsResetting = false;
            }
        }
        time -= Time.fixedDeltaTime;
    }
}
