using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] float _bulletSpeed = 10;
    [SerializeField] float defaultPickupSpawnCooldown = 7;
    [SerializeField] float chanceForPickupToSpawnPerFrame = 5; // out of 1000%

    // References
    [SerializeField] GameObject lifePickup = null;
    [SerializeField] GameObject speedPickup = null;

    // Public properties
    public static GameManager instance => _instance;
    public float bulletSpeed => _bulletSpeed;
    public GameObject leftPlayer => _leftPlayer;
    public GameObject rightPlayer => _rightPlayer;

    // Private variables and references
    static GameManager _instance = null;
    GameObject _leftPlayer = null;
    GameObject _rightPlayer = null;
    float pickupSpawnCooldown = 0;
    #endregion

    // Public methods
    #region Public methods
    public void GameOver()
    {
        Debug.Log("Game Over!");
    }
    #endregion

    // Inherited methods
    #region MyRegion
    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _leftPlayer = GameObject.FindGameObjectWithTag("LeftPlayer");
        _rightPlayer = GameObject.FindGameObjectWithTag("RightPlayer");
    }

    private void FixedUpdate()
    {
        if (pickupSpawnCooldown > 0)
        {
            pickupSpawnCooldown -= Time.deltaTime;
        }
        else
        {
            int randomNumber = Random.Range(0, 1000);
            if (randomNumber < chanceForPickupToSpawnPerFrame)
            {
                int randomPickup = Random.Range(0, 2);
                if (randomPickup == 0)
                {
                    Instantiate(lifePickup, new Vector3(0, 6, 0), new Quaternion());
                }
                else
                {
                    Instantiate(speedPickup, new Vector3(0, 6, 0), new Quaternion());
                }
                pickupSpawnCooldown = defaultPickupSpawnCooldown;
            }
        }
        
    }
    #endregion
}
