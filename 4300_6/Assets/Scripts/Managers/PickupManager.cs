using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] float _speedupBulletSpeedMultiplier = 3;
    [SerializeField] float _speedupPickupTime = 5;
    [SerializeField] float stormDuration = 7;
    [SerializeField] float chanceForLightningBoltToSpawnPerSecond = 3;
    [SerializeField] float chanceForPickupToSpawnPerSecond = 10;
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
    public float speedupBulletSpeed => _speedupBulletSpeedMultiplier;
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
    public void Pickup_Jetpack(string tag)
    {
        if (tag == "Player1")
        {
            GameManager.instance.player1.SetMovementMode(Player.MovementMode.JETPACK);
        }
        else
        {
            // GameManager.instance.player2.EnterJetpackMode();
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
