using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Classes and Enums
    public enum SoundType
    {
        SOUND_1
    }

    // References
    [SerializeField] AudioSource sound_1 = null;

    // Public methods
    public void PlayOnce(SoundType type)
    {
        switch (type)
        {
            case SoundType.SOUND_1:
                {
                    sound_1.PlayOneShot(sound_1.clip);
                }break;
        }
    }
}
