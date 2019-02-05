using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateManager : MonoBehaviour
{
    // Inspector variables
    [SerializeField] float _crateSpeedLimit = 5;
    [SerializeField] float chanceToSpawnCratePerSecond = 10;

    // References
    [SerializeField] GameObject cratePrefab = null;
    static CrateManager _instance = null;

    // Public properties
    public static CrateManager instance => _instance;
    public float speedLimit => _crateSpeedLimit;

    // Private variables
    float timer = 1;

    // Inherited methods
    private void Awake()
    {
        _instance = this;
    }

    private void FixedUpdate()
    {
        if (timer < 0)
        {
            float randomNumber = Random.Range(0f, 100f);
            if (randomNumber <= chanceToSpawnCratePerSecond)
            {
                float randomPosition = Random.Range(-GameManager.instance.gameViewHorizontalDistanceInMeters / 2 + 0.5f, GameManager.instance.gameViewHorizontalDistanceInMeters / 2 - 0.5f);
                Instantiate(cratePrefab, new Vector3(randomPosition, GameManager.instance.gameViewVerticalDistanceInMeters, 0), new Quaternion());
            }
            timer = 1;
        }

        timer -= Time.fixedDeltaTime;
    }
}
