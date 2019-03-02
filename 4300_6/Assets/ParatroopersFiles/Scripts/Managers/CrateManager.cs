using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateManager : MonoBehaviour
{
    // Inspector variables
    [SerializeField] float _crateSpeedLimit = 5;
    [SerializeField] float chanceToSpawnCratePerSecond = 10;
    [SerializeField] float spawnCooldown = 1f;

    // References
    [SerializeField] GameObject cratePrefab = null;
    static CrateManager _instance = null;

    // Private variables
    const float HORIZONTAL_BUFFER = 0.5f;
    const float CRATE_VERTICAL_SIZE = 2f;
    float timer;

    // Public properties
    public static CrateManager Instance => _instance;
    public float SpeedLimit => _crateSpeedLimit;

    // Inherited methods
    private void Awake()
    {
        _instance = this;
    }
    private void FixedUpdate()
    {
        // Run a check to possibly spawn a new crate if the cooldown is down.
        if (timer < 0)
        {
            float randomNumber = Random.Range(0f, 100f);
            if (randomNumber <= chanceToSpawnCratePerSecond)
            {
                float randomPosition = Random.Range(-GameManager.Instance.GameViewHorizontalDistanceInMeters / 2 + HORIZONTAL_BUFFER, GameManager.Instance.GameViewHorizontalDistanceInMeters / 2 - HORIZONTAL_BUFFER);
                Instantiate(cratePrefab, new Vector3(randomPosition, GameManager.Instance.GameViewVerticalDistanceInMeters/2 + CRATE_VERTICAL_SIZE, 0), new Quaternion());
            }
            timer = spawnCooldown;
        }
        timer -= Time.fixedDeltaTime;
    }
}
