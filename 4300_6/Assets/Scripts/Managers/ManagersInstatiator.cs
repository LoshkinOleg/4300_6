using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagersInstatiator : MonoBehaviour
{
    [SerializeField] GameObject gameManagerPrefab = null;

    private void Awake()
    {
        if (GameManager.instance == null)
        {
            Instantiate(gameManagerPrefab);
        }
    }
}
