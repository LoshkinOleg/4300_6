using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Public references
    static GameManager instance => instance;

    // Private variables and references
    static GameManager _instance = null;

    private void Awake()
    {
        _instance = this;
    }
}
