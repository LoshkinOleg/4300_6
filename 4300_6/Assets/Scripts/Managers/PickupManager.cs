using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] float _speedupBulletSpeed = 20;
    [SerializeField] float _speedupPickupTime = 5;
    [SerializeField] float stormDuration = 7;
    [SerializeField] float chanceForLightningBoltToSpawnPerSecond = 3;
    [SerializeField] float chanceForPickupToSpawnPerSecond = 10;
    [SerializeField] float healthPickupValue = 0.5f;
    [SerializeField] float _pickupSpeedLimit = 3;

    // References
    [SerializeField] GameObject lifePickupPrefab = null;
    [SerializeField] GameObject healthPickupPrefab = null;
    [SerializeField] GameObject shieldPickupPrefab = null;
    [SerializeField] GameObject speedupPickupPrefab = null;
    [SerializeField] GameObject stormPickupPrefab = null;
    [SerializeField] GameObject jetpackPickupPrefab = null;
    [SerializeField] GameObject stormPrefab = null;
    [SerializeField] GameObject lightningBoltPrefab = null;
    [SerializeField] GameObject shieldPrefab = null;
    static PickupManager _instance = null;

    // Public properties
    static public PickupManager instance => _instance;
    public float pickupSpeedLimit => _pickupSpeedLimit;
    public float speedupBulletSpeed => _speedupBulletSpeed;
    public float speedupPickupTime => _speedupPickupTime;

    // Private variables
    float gameViewHorizontalDistanceInMeters = 17.78f;
    #endregion

    // Public methods
    #region Public methods
    public void Pickup_Life(string tag)
    {
        if (tag == "Player1")
        {
            GameManager.instance.player1.lives++;
        }
        else
        {
            // GameManager.instance.player2.lives++;
        }
    }
    public void Pickup_Health(string tag)
    {
        if (tag == "Player1")
        {
            GameManager.instance.player1.ModifyHealth(healthPickupValue);
        }
        else
        {
            // GameManager.instance.player2.ModifyHealth(healthPickupValue);
        }
    }
    public void Pickup_Shield(string tag)
    {
        if (tag == "Player1")
        {
            Instantiate(shieldPrefab).GetComponent<Shield>().target = GameManager.instance.player1.gameObject;
        }
        else
        {
            // Instantiate(shieldPrefab).GetComponent<Shield>().target = GameManager.instance.player2.gameObject;
        }
    }
    public void Pickup_Speedup(string tag)
    {
        if (tag == "Player1")
        {
            GameManager.instance.player1.SpeedBulletsUp();
        }
        else
        {
            // GameManager.instance.player2.SpeedBulletsUp();
        }
    }
    public void Pickup_Storm(string tag)
    {
        if (tag == "Player1")
        {
            // Instantiate(stormPrefab).GetComponent<Storm>().target = GameManager.instance.player2.gameObject;
        }
        else
        {
            Instantiate(stormPrefab).GetComponent<Storm>().target = GameManager.instance.player1.gameObject;
        }
    }
    public void Pickup_Jetpack(string tag)
    {
        if (tag == "Player1")
        {
            GameManager.instance.player1.EnterJetpackMode();
        }
        else
        {
            // GameManager.instance.player2.EnterJetpackMode();
        }
    }
    #endregion

    // Private methods
    #region Private methods
    IEnumerator CheckForPickupSpawn() // Generates a random number and if said number is smaller than chanceForPickupToSpawnPerSecond, spawns a random pickup.
    {
        yield return new WaitForSeconds(1);

        int randomNumber = Random.Range(0, 100);
        if (randomNumber <= chanceForPickupToSpawnPerSecond)
        {
            int randomPickup = Random.Range(0, 7);
        }
    }
    IEnumerator CheckForLightningBoltSpawn() // Generates a random number and if said number is smaller than chanceForLightningBoltToSpawnPerSecond, spawns a lightning bolt at a random X location.
    {
        yield return new WaitForSeconds(1);


    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void Awake()
    {
        _instance = this;
    }

    private void FixedUpdate()
    {
        StartCoroutine(CheckForPickupSpawn());
    }
    #endregion
}
