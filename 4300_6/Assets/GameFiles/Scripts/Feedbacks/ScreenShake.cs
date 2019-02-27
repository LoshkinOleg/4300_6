using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] [Range(0, 2)] float[] firingScreenShakeIntensities = new float[(int)Weapon.MINIGUN + 1];

    CinemachineVirtualCamera vCamera = null;
    float time;
    bool amplitudeNeedsResetting;

    public void ShakeScreen(Weapon type)
    {
        time = firingScreenShakeIntensities[(int)type];
        amplitudeNeedsResetting = true;
    }

    void Start()
    {
        vCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void FixedUpdate()
    {
        if (time > 0)
        {
            vCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = time * 3;
            vCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = time * 3;
        }
        else
        {
            if (amplitudeNeedsResetting)
            {
                vCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
                vCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
                amplitudeNeedsResetting = false;
            }
        }

        time -= Time.fixedDeltaTime;
    }
}
