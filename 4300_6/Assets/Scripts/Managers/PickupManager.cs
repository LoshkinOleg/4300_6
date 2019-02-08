using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] float _speedupPickupTime = 5;
    [SerializeField] float chanceForPickupToSpawnPerSecond = 100;
    [SerializeField] float healthPickupValue = 0.5f;
    [SerializeField] float _pickupSpeedLimit = 3;
    [SerializeField] float _jetpackDuration = 10;
    [SerializeField] float _jetpackVelocity = 7;
    [SerializeField] float _firerate_pistol = 4;
    [SerializeField] float _firerate_shotgun = 1;
    [SerializeField] float _firerate_sniper = 1;
    [SerializeField] float _firerate_bazooka = 0.5f;
    [SerializeField] float _firerate_minigun = 20;
    [SerializeField] float _bulletSpeed_pistol = 10;
    [SerializeField] float _bulletSpeed_shotgun = 10;
    [SerializeField] float _bulletSpeed_sniper = 40;
    [SerializeField] float _bulletSpeed_bazooka = 5;
    [SerializeField] float _bulletSpeed_minigun = 10;
    [SerializeField] float _bulletDamage_pistol = 0.01f;
    [SerializeField] float _bulletDamage_shotgun = 0.1f;
    [SerializeField] float _bulletDamage_sniper = 0.34f;
    [SerializeField] float _bulletDamage_bazooka = 0.2f;
    [SerializeField] float _bulletDamage_minigun = 0.01f;
    [SerializeField] float _bulletSpread_pistol = 3f;
    [SerializeField] float _bulletSpread_shotgun = 0f;
    [SerializeField] float _bulletSpread_sniper = 0f;
    [SerializeField] float _bulletSpread_bazooka = 0f;
    [SerializeField] float _bulletSpread_minigun = 7f;

    // References
    [SerializeField] GameObject pickupPrefab_life = null;
    [SerializeField] GameObject pickupPrefab_health = null;
    [SerializeField] GameObject pickupPrefab_shield = null;
    [SerializeField] GameObject pickupPrefab_speedup = null;
    [SerializeField] GameObject pickupPrefab_jetpack = null;
    [SerializeField] GameObject pickupPrefab_shotgun = null;
    [SerializeField] GameObject pickupPrefab_sniper = null;
    [SerializeField] GameObject pickupPrefab_bazooka = null;
    [SerializeField] GameObject pickupPrefab_minigun = null;
    [SerializeField] GameObject shieldPrefab = null;
    static PickupManager _instance = null;

    // Public properties
    static public PickupManager instance => _instance;
    public float pickupSpeedLimit => _pickupSpeedLimit;
    public float speedupPickupTime => _speedupPickupTime;
    public float jetpackDuration => _jetpackDuration;
    public float jetpackVelocity => _jetpackVelocity;
    public float firerate_pistol => _firerate_pistol;
    public float firerate_shotgun => _firerate_shotgun;
    public float firerate_sniper => _firerate_sniper;
    public float firerate_bazooka => _firerate_bazooka;
    public float firerate_minigun => _firerate_minigun;
    public float bulletSpeed_pistol => _bulletSpeed_pistol;
    public float bulletSpeed_shotgun => _bulletSpeed_shotgun;
    public float bulletSpeed_sniper => _bulletSpeed_sniper;
    public float bulletSpeed_bazooka => _bulletSpeed_bazooka;
    public float bulletSpeed_minigun => _bulletSpeed_minigun;
    public float bulletDamage_pistol => _bulletDamage_pistol;
    public float bulletDamage_shotgun => _bulletDamage_shotgun;
    public float bulletDamage_sniper => _bulletDamage_sniper;
    public float bulletDamage_bazooka => _bulletDamage_bazooka;
    public float bulletDamage_minigun => _bulletDamage_minigun;
    public float bulletSpread_pistol => _bulletSpread_pistol;
    public float bulletSpread_shotgun => _bulletSpread_shotgun;
    public float bulletSpread_sniper => _bulletSpread_sniper;
    public float bulletSpread_bazooka => _bulletSpread_bazooka;
    public float bulletSpread_minigun => _bulletSpread_minigun;
    #endregion

    // Private variables
    bool runningSpawnCheck = false;
    int numberOfPickupTypes = (int)Pickup.Type.LIFE + 1;

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
            GameManager.instance.player2.lives++;
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
            GameManager.instance.player2.ModifyHealth(healthPickupValue);
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
            Instantiate(shieldPrefab).GetComponent<Shield>().target = GameManager.instance.player2.gameObject;
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
            GameManager.instance.player2.SpeedBulletsUp();
        }
    }
    public void Pickup_Jetpack(string tag)
    {
        if (tag == "Player1")
        {
            GameManager.instance.player1.SetMovementMode(Player.MovementMode.JETPACK);
        }
        else
        {
            GameManager.instance.player2.SetMovementMode(Player.MovementMode.JETPACK);
        }
    }
    public void Pickup_Shotgun(string tag)
    {
        if (tag == "Player1")
        {
            GameManager.instance.player1.currentWeapon = Player.Weapon.SHOTGUN;
        }
        else
        {
            GameManager.instance.player2.currentWeapon = Player.Weapon.SHOTGUN;
        }
    }
    public void Pickup_Sniper(string tag)
    {
        if (tag == "Player1")
        {
            GameManager.instance.player1.currentWeapon = Player.Weapon.SNIPER;
        }
        else
        {
            GameManager.instance.player2.currentWeapon = Player.Weapon.SNIPER;
        }
    }
    public void Pickup_Bazooka(string tag)
    {
        if (tag == "Player1")
        {
            GameManager.instance.player1.currentWeapon = Player.Weapon.BAZOOKA;
        }
        else
        {
            GameManager.instance.player2.currentWeapon = Player.Weapon.BAZOOKA;
        }
    }
    public void Pickup_Minigun(string tag)
    {
        if (tag == "Player1")
        {
            GameManager.instance.player1.currentWeapon = Player.Weapon.MINIGUN;
        }
        else
        {
            GameManager.instance.player2.currentWeapon = Player.Weapon.BAZOOKA;
        }
    }
    #endregion

    // Private methods
    #region Private methods
    IEnumerator CheckForPickupSpawn() // Generates a random number and if said number is smaller than chanceForPickupToSpawnPerSecond, spawns a random pickup.
    {
        runningSpawnCheck = true;

        yield return new WaitForSeconds(1);

        int randomNumber = Random.Range(0, 100);
        if (randomNumber <= chanceForPickupToSpawnPerSecond)
        {
            // Pick a random pickup to spawn. Possibly add weights later.
            Pickup.Type randomPickup = (Pickup.Type)Random.Range(0, numberOfPickupTypes);
            float horizontalPosition = Random.Range(-GameManager.instance.gameViewHorizontalDistanceInMeters/2 + 1f, GameManager.instance.gameViewHorizontalDistanceInMeters/2 - 1f);
            switch (randomPickup)
            {
                case Pickup.Type.BAZOOKA:
                    {
                        Instantiate(pickupPrefab_bazooka, new Vector3(horizontalPosition, GameManager.instance.gameViewVerticalDistanceInMeters/2 + 5f, 0), new Quaternion());
                    }break;
                case Pickup.Type.HEALTH:
                    {
                        Instantiate(pickupPrefab_health, new Vector3(horizontalPosition, GameManager.instance.gameViewVerticalDistanceInMeters / 2 + 5f, 0), new Quaternion());
                    }
                    break;
                case Pickup.Type.JETPACK:
                    {
                        Instantiate(pickupPrefab_jetpack, new Vector3(horizontalPosition, GameManager.instance.gameViewVerticalDistanceInMeters / 2 + 5f, 0), new Quaternion());
                    }
                    break;
                case Pickup.Type.LIFE:
                    {
                        Instantiate(pickupPrefab_life, new Vector3(horizontalPosition, GameManager.instance.gameViewVerticalDistanceInMeters / 2 + 5f, 0), new Quaternion());
                    }
                    break;
                case Pickup.Type.MINIGUN:
                    {
                        Instantiate(pickupPrefab_minigun, new Vector3(horizontalPosition, GameManager.instance.gameViewVerticalDistanceInMeters / 2 + 5f, 0), new Quaternion());
                    }
                    break;
                case Pickup.Type.SHIELD:
                    {
                        Instantiate(pickupPrefab_shield, new Vector3(horizontalPosition, GameManager.instance.gameViewVerticalDistanceInMeters / 2 + 5f, 0), new Quaternion());
                    }
                    break;
                case Pickup.Type.SHOTGUN:
                    {
                        Instantiate(pickupPrefab_shotgun, new Vector3(horizontalPosition, GameManager.instance.gameViewVerticalDistanceInMeters / 2 + 5f, 0), new Quaternion());
                    }
                    break;
                case Pickup.Type.SNIPER:
                    {
                        Instantiate(pickupPrefab_sniper, new Vector3(horizontalPosition, GameManager.instance.gameViewVerticalDistanceInMeters / 2 + 5f, 0), new Quaternion());
                    }
                    break;
                case Pickup.Type.SPEED_UP:
                    {
                        Instantiate(pickupPrefab_speedup, new Vector3(horizontalPosition, GameManager.instance.gameViewVerticalDistanceInMeters / 2 + 5f, 0), new Quaternion());
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
