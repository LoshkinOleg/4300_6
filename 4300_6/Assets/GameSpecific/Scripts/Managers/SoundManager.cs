using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SoundManager : MonoBehaviour
{
    // Attributes
    #region Attributes
    static SoundManager _instance = null;

    // Public properties
    static public SoundManager Instance => _instance;

    // Private variables
    IDictionary<string, StudioEventEmitter> sounds = new Dictionary<string, StudioEventEmitter>();
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
        StudioEventEmitter[] emitters = GetComponents<StudioEventEmitter>();
        foreach (var item in emitters)
        {
            sounds.Add(item.Event.ToString().Remove(0,7), item);
        }
    }
    #endregion
}
