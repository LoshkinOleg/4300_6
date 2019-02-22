using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenShake : MonoBehaviour
{
    CinemachineVirtualCamera vCamera = null;
    float time;
    bool amplitudeNeedsResetting;

    public void ShakeScreen(float time)
    {
        this.time = time;
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
                vCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 1;
                vCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 1;
                amplitudeNeedsResetting = false;
            }
        }

        time -= Time.fixedDeltaTime;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ShakeScreen(2);
        }
    }
}
