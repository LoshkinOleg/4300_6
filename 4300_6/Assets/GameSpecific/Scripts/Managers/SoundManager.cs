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

    // Private variables
    static SoundManager _instance = null;
    IDictionary<string, FMOD.Studio.EventInstance> sounds = new Dictionary<string, FMOD.Studio.EventInstance>();
    #endregion

    // Public methods
    #region Public methods
    public void PlayShortSound(string name)
    {
        sounds[name].start();
    }
    public void PlayLoopingSound(string name)
    {
        sounds[name].start();
    }
    public void StopLoopingSound(string name)
    {
        sounds[name].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        for (int i = 0; i < paths.Length; i++)
        {
            sounds[paths[i].Remove(0,7)] = RuntimeManager.CreateInstance(paths[i]);
        }
    }
    #endregion
}
