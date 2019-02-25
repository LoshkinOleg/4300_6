using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SoundManager : MonoBehaviour
{
    // Attributes
    #region Attributes
    [EventRef] [SerializeField] string[] paths = new string[0];

    // Public properties
    static public SoundManager Instance => _instance;
    public float minigunSpinupTime => _minigunSpinupTime;
    public float minigunSlowdownTime => _minigunSlowdownTime;
    public float outOfAmmoTime => _outOfAmmoTime;
    public float reloadTime => _reloadTime;

    // Private variables
    static SoundManager _instance = null;
    IDictionary<string, FMOD.Studio.EventInstance> sounds = new Dictionary<string, FMOD.Studio.EventInstance>();
    float _minigunSpinupTime;
    float _minigunSlowdownTime;
    float _outOfAmmoTime;
    float outOfAmmoTimer;
    float _reloadTime;
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

        for (int i = 0; i < paths.Length; i++)
        {
            sounds[paths[i].Remove(0,7)] = RuntimeManager.CreateInstance(paths[i]);
        }
        
        // Setup minigun time properties.
        FMOD.Studio.EventDescription description;
        int duration;

        sounds["minigun_spinup"].getDescription(out description);
        description.getLength(out duration);
        _minigunSpinupTime = (float)duration / 1000f;

        sounds["minigun_slowdown"].getDescription(out description);
        description.getLength(out duration);
        _minigunSlowdownTime = (float)duration / 1000f;

        sounds["out_of_ammo"].getDescription(out description);
        description.getLength(out duration);
        _outOfAmmoTime = (float)duration / 1000f + 0.5f; // The 0.5f keeps the sound from looping the same 0.1 seconds constantly resulting in some very bad auditive feedback.

        sounds["shotgun_reloading"].getDescription(out description);
        description.getLength(out duration);
        _reloadTime = (float)duration / 1000f;
    }
    private void Update()
    {
        outOfAmmoTimer -= Time.deltaTime;
    }
    #endregion
}
