using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Classes and Enums
    public enum SoundType
    {
        BULLET_DESTRUCTION,
        PICKUP,
        PLAYER_HIT,
        SHIELD_HIT,
        FIRE,
        THUNDER,
        PARACHUTE_OPEN,
        PARACHUTE_CLOSED
    }

    // Attributes
    #region Attributes
    // References
    [SerializeField] AudioSource sound_bulletDestructuion = null;
    [SerializeField] AudioSource sound_pickup = null;
    [SerializeField] AudioSource sound_playerHit = null;
    [SerializeField] AudioSource sound_shieldHit = null;
    [SerializeField] AudioSource sound_fire = null;
    [SerializeField] AudioSource sound_thunder = null;
    [SerializeField] AudioSource sound_parachuteOpen = null;
    [SerializeField] AudioSource sound_parachuteClosed = null;
    static SoundManager _instance = null;

    // Public properties
    static public SoundManager instance => _instance;
    #endregion

    // Public methods
    public void PlayOnce(SoundType type)
    {
        switch (type)
        {
            case SoundType.BULLET_DESTRUCTION:
                {
                    sound_bulletDestructuion.PlayOneShot(sound_bulletDestructuion.clip);
                }break;
            case SoundType.PICKUP:
                {
                    sound_pickup.PlayOneShot(sound_pickup.clip);
                }
                break;
            case SoundType.PLAYER_HIT:
                {
                    sound_playerHit.PlayOneShot(sound_playerHit.clip);
                }
                break;
            case SoundType.SHIELD_HIT:
                {
                    sound_shieldHit.PlayOneShot(sound_shieldHit.clip);
                }
                break;
            case SoundType.FIRE:
                {
                    sound_fire.PlayOneShot(sound_fire.clip);
                }
                break;
            case SoundType.THUNDER:
                {
                    sound_thunder.PlayOneShot(sound_thunder.clip);
                }
                break;
            case SoundType.PARACHUTE_OPEN:
                {
                    sound_parachuteOpen.PlayOneShot(sound_parachuteOpen.clip);
                }
                break;
            case SoundType.PARACHUTE_CLOSED:
                {
                    sound_parachuteClosed.PlayOneShot(sound_parachuteClosed.clip);
                }
                break;
        }
    }

    // Inherited methods
    #region Inherited methods
    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion
}
