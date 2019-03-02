using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

// NOTE: Really not the most efficient manner of hanlding sounds but I don't really have time to rewrite it.

public class SoundManager : MonoBehaviour
{
    // Attributes
    #region Attributes
    [EventRef] [SerializeField] string[] paths = new string[0];

    // Private variables
    IDictionary<string, FMOD.Studio.EventInstance> sounds = new Dictionary<string, FMOD.Studio.EventInstance>();
    static SoundManager _instance = null;
    float _minigunSpinupTime;
    float _minigunSlowdownTime;
    float _outOfAmmoTime;
    float _reloadTime;
    float outOfAmmoTimer;
    #endregion

    // Public properties
    #region MyRegion
    static public SoundManager Instance => _instance;
    public float MinigunSpinupTime => _minigunSpinupTime;
    public float MinigunSlowdownTime => _minigunSlowdownTime;
    public float OutOfAmmoTime => _outOfAmmoTime;
    public float ReloadTime => _reloadTime;
    #endregion

    // Public methods
    #region Public methods
    public void PlaySound(string name)
    {
        sounds[name].start();
    }
    public void PlayOutOfAmmoSound()
    {
        if (outOfAmmoTimer < 0)
        {
            sounds["out_of_ammo"].start();
            outOfAmmoTimer = _outOfAmmoTime;
        }
    }
    public void StopSound(string name)
    {
        sounds[name].stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
    public void StopAllSounds()
    {
        foreach (var item in sounds)
        {
            item.Value.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void OnDestroy()
    {
        foreach (var item in sounds)
        {
            item.Value.release();
        }
    }
    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Create easily writeable keys and fill the dictionary.
        for (int i = 0; i < paths.Length; i++)
        {
            sounds[paths[i].Remove(0,7)] = RuntimeManager.CreateInstance(paths[i]);
        }
        
        // Figure out clip durations we need.
        FMOD.Studio.EventDescription description;
        int duration;

        sounds["minigun_spinup"].getDescription(out description);
        description.getLength(out duration);
        _minigunSpinupTime = duration / 1000f; // Returned duration is in milliseconds but we need seconds, hence the /1000f.

        sounds["minigun_slowdown"].getDescription(out description);
        description.getLength(out duration);
        _minigunSlowdownTime = duration / 1000f;

        sounds["out_of_ammo"].getDescription(out description);
        description.getLength(out duration);
        _outOfAmmoTime = duration / 1000f + 0.5f; // The 0.5f keeps the sound from looping the same 0.1 seconds constantly resulting in some very bad auditive feedback.

        sounds["shotgun_reloading"].getDescription(out description);
        description.getLength(out duration);
        _reloadTime = duration / 1000f;
    }
    private void Update()
    {
        outOfAmmoTimer -= Time.deltaTime;
    }
    #endregion
}
