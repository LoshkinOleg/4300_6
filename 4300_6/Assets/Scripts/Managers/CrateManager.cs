using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateManager : MonoBehaviour
{
    // References
    static CrateManager _instance = null;

    // Public properties
    public static CrateManager instance => _instance;

    private void Awake()
    {
        _instance = this;
    }
}
