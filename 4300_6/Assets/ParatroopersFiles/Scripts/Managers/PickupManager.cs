using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] float chanceForPickupToSpawnPerSecond = 100;
    [SerializeField] float _speedupPickupTime = 5;
    [SerializeField] float _speedupMultiplier = 2f;
    [SerializeField] int healthPickupValue = 7;
    [SerializeField] float _pickupSpeedLimit = 3;
    [SerializeField] float pickupSpawnCooldown = 1f;

    // Prefabs
    [SerializeField] GameObject pickupPrefab_health = null;
    [SerializeField] GameObject pickupPrefab_shield = null;
    [SerializeField] GameObject pickupPrefab_speedup = null;
    [SerializeField] GameObject pickupPrefab_shotgun = null;
    [SerializeField] GameObject pickupPrefab_sniper = null;
    [SerializeField] GameObject pickupPrefab_bazooka = null;
    [SerializeField] GameObject pickupPrefab_minigun = null;
    [SerializeField] GameObject shieldPrefab = null;
    
    // Private variables
    static PickupManager _instance = null;
    bool runningSpawnCheck = false;
    const float HORIZONTAL_SPAWN_BUFFER = 1f;
    const float PICKUP_VERTICAL_SIZE = 1.5f;
    #endregion

    // Public properties
    #region MyRegion
    static public PickupManager Instance => _instance;
    public float PickupSpeedLimit => _pickupSpeedLimit;
    public float SpeedupPickupTime => _speedupPickupTime;
    public float SpeedupMultiplier => _speedupMultiplier;
    #endregion

    // Public methods
    #region Public methods
    public void Pickup_Health(string tag)
    {
        int player = tag == "Player1" ? 0 : 1;
        GameManager.Instance.Players[player].ModifyHealth(healthPickupValue);
        SoundManager.Instance.PlaySound("pickup");
    }
    public void Pickup_Shield(string tag)
    {
        int player = tag == "Player1" ? 0 : 1;
        Instantiate(shieldPrefab).GetComponent<Shield>().target = GameManager.Instance.Players[player].gameObject;
        SoundManager.Instance.PlaySound("pickup");
    }
    public void Pickup_Speedup(string tag)
    {
        int player = tag == "Player1" ? 0 : 1;
        GameManager.Instance.Players[player].SpeedBulletsUp();
        SoundManager.Instance.PlaySound("pickup");
    }
    public void Pickup_Shotgun(string tag)
    {
        int player = tag == "Player1" ? 0 : 1;
        GameManager.Instance.Players[player].SwitchToWeapon(Weapon.SHOTGUN);
        SoundManager.Instance.PlaySound("shotgun_reloading");
    }
    public void Pickup_Sniper(string tag)
    {
        int player = tag == "Player1" ? 0 : 1;
        GameManager.Instance.Players[player].SwitchToWeapon(Weapon.SNIPER);
        SoundManager.Instance.PlaySound("shotgun_reloading");
    }
    public void Pickup_Bazooka(string tag)
    {
        int player = tag == "Player1" ? 0 : 1;
        GameManager.Instance.Players[player].SwitchToWeapon(Weapon.BAZOOKA);
        SoundManager.Instance.PlaySound("shotgun_reloading");
    }
    public void Pickup_Minigun(string tag)
    {
        int player = tag == "Player1" ? 0 : 1;
        GameManager.Instance.Players[player].SwitchToWeapon(Weapon.MINIGUN);
        SoundManager.Instance.PlaySound("shotgun_reloading");
    }
    #endregion

    // Private methods
    #region Private methods
    IEnumerator CheckForPickupSpawn() // Generates a random number and if said number is smaller than chanceForPickupToSpawnPerSecond, spawns a random pickup.
    {
        runningSpawnCheck = true;
        yield return new WaitForSeconds(pickupSpawnCooldown);

        int randomNumber = Random.Range(0, 100);
        if (randomNumber <= chanceForPickupToSpawnPerSecond)
        {
            // Pick a random pickup to spawn.
            randomNumber = Random.Range(0,100);
            Pickup.Type randomPickup;
            if (randomNumber > 90) // Spawn a minigun
            {
                randomPickup = Pickup.Type.MINIGUN;
            }
            else if (randomNumber > 50) // Spawn a Shield, sniper or bazooka
            {
                randomNumber = Random.Range(0, 100);
                if (randomNumber > 66)
                {
                    randomPickup = Pickup.Type.SHIELD;
                }
                else if (randomNumber > 33)
                {
                    randomPickup = Pickup.Type.SNIPER;
                }
                else
                {
                    randomPickup = Pickup.Type.BAZOOKA;
                }
            }
            else // Spawn HP, shotgun or speedup
            {
                randomNumber = Random.Range(0, 100);
                if (randomNumber > 66)
                {
                    randomPickup = Pickup.Type.HEALTH;
                }
                else if (randomNumber > 33)
                {
                    randomPickup = Pickup.Type.SHOTGUN;
                }
                else
                {
                    randomPickup = Pickup.Type.SPEED_UP;
                }
            }

            // Pick a random position.
            float horizontalPosition = Random.Range(-GameManager.Instance.GameViewHorizontalDistanceInMeters/2 + HORIZONTAL_SPAWN_BUFFER, GameManager.Instance.GameViewHorizontalDistanceInMeters/2 - HORIZONTAL_SPAWN_BUFFER);
            switch (randomPickup)
            {
                case Pickup.Type.BAZOOKA:
                    {
                        Instantiate(pickupPrefab_bazooka, new Vector3(horizontalPosition, GameManager.Instance.GameViewVerticalDistanceInMeters/2 + PICKUP_VERTICAL_SIZE, 0), new Quaternion());
                    }break;
                case Pickup.Type.HEALTH:
                    {
                        Instantiate(pickupPrefab_health, new Vector3(horizontalPosition, GameManager.Instance.GameViewVerticalDistanceInMeters / 2 + PICKUP_VERTICAL_SIZE, 0), new Quaternion());
                    }
                    break;
                case Pickup.Type.MINIGUN:
                    {
                        Instantiate(pickupPrefab_minigun, new Vector3(horizontalPosition, GameManager.Instance.GameViewVerticalDistanceInMeters / 2 + PICKUP_VERTICAL_SIZE, 0), new Quaternion());
                    }
                    break;
                case Pickup.Type.SHIELD:
                    {
                        Instantiate(pickupPrefab_shield, new Vector3(horizontalPosition, GameManager.Instance.GameViewVerticalDistanceInMeters / 2 + PICKUP_VERTICAL_SIZE, 0), new Quaternion());
                    }
                    break;
                case Pickup.Type.SHOTGUN:
                    {
                        Instantiate(pickupPrefab_shotgun, new Vector3(horizontalPosition, GameManager.Instance.GameViewVerticalDistanceInMeters / 2 + PICKUP_VERTICAL_SIZE, 0), new Quaternion());
                    }
                    break;
                case Pickup.Type.SNIPER:
                    {
                        Instantiate(pickupPrefab_sniper, new Vector3(horizontalPosition, GameManager.Instance.GameViewVerticalDistanceInMeters / 2 + PICKUP_VERTICAL_SIZE, 0), new Quaternion());
                    }
                    break;
                case Pickup.Type.SPEED_UP:
                    {
                        Instantiate(pickupPrefab_speedup, new Vector3(horizontalPosition, GameManager.Instance.GameViewVerticalDistanceInMeters / 2 + PICKUP_VERTICAL_SIZE, 0), new Quaternion());
                    }
                    break;
            }
        }
        runningSpawnCheck = false;
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
        if (!runningSpawnCheck)
        {
            StartCoroutine(CheckForPickupSpawn());
        }
    }
    #endregion
}
