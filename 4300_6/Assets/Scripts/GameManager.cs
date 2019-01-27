using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Inspector variables
    // Bullet Speedup/SlowDown related
    [SerializeField] float _defaultBulletSpeed = 10;
    [SerializeField] float speedupBulletSpeed = 20;
    [SerializeField] float speedupPickupTime = 5;
    [SerializeField] float slowdownBulletSpeed = 5;
    [SerializeField] float slowdownPickupTime = 5;
    // Storm related
    [SerializeField] float stormDuration = 7;
    [SerializeField] float chanceForLightningBoltToSpawnPerSecond = 3;
    // Health pickup related
    [SerializeField] float _healthPickupValue = 0.5f;
    // Generally pickup related
    [SerializeField] float chanceForPickupToSpawnPerSecond = 10;
    [SerializeField] float defaultPickupSpawnCooldown = 4;

    // References
    [SerializeField] GameObject lifePickupPrefab = null;
    [SerializeField] GameObject shieldPickupPrefab = null;
    [SerializeField] GameObject speedupPickupPrefab = null;
    [SerializeField] GameObject slowdownPickupPrefab = null;
    [SerializeField] GameObject stormPickupPrefab = null;
    [SerializeField] GameObject lightningBoltPrefab = null;
    [SerializeField] GameObject shieldPrefab = null;
    static GameManager _instance = null;
    GameObject _leftPlayer = null;
    GameObject _rightPlayer = null;

    // Public properties
    public static GameManager instance => _instance;
    public float defaultBulletSpeed => _defaultBulletSpeed;
    public GameObject leftPlayer => _leftPlayer;
    public GameObject rightPlayer => _rightPlayer;
    public float healthPickupValue => _healthPickupValue;
    public float leftPlayerBulletSpeed => _leftPlayerBulletSpeed;
    public float rightPlayerBulletSpeed => _rightPlayerBulletSpeed;

    // Private variables and references
    // Bullet Speedup/Slowdown related
    float _leftPlayerBulletSpeed;
    float _rightPlayerBulletSpeed;
    bool leftPlayerIsSpeedup = false;
    bool rightPlayerIsSpeedup = false;
    float leftPlayerSpeedupTimer = 0;
    float rightPlayerSpeedupTimer = 0;
    bool leftPlayerIsSlowedDown = false;
    bool rightPlayerIsSlowedDown = false;
    float leftPlayerSlowDownTimer = 0;
    float rightPlayerSlowDownTimer = 0;
    // Storm related
    float stormTimer = 0;
    bool stormTriggered = false;
    // General pickup related
    bool spawnPickups = true;
    float pickupSpawnCooldown = 0;
    bool decidingWhetherToSpawnPickup = false;
    // Other general variables
    GameObject loser = null;
    float gameViewHorizontalDistanceInMeters = 17.78f;
    #endregion

    // Public methods
    #region Public methods
    public void GameOver(GameObject loser)
    {
        this.loser = loser;
        SceneManager.LoadScene("Score");
    }
    public void SpeedUpBullets(GameObject player)
    {
        if (player.tag == "LeftPlayer")
        {
            leftPlayerIsSpeedup = true;
            leftPlayerSpeedupTimer = speedupPickupTime;
            _leftPlayerBulletSpeed = speedupBulletSpeed;
        }
        else
        {
            rightPlayerIsSpeedup = true;
            rightPlayerSpeedupTimer = speedupPickupTime;
            _rightPlayerBulletSpeed = speedupBulletSpeed;
        }
    }
    public void SlowDownBullets(GameObject player)
    {
        if (player.tag == "LeftPlayer")
        {
            rightPlayerIsSlowedDown = true;
            rightPlayerSlowDownTimer = slowdownPickupTime;
            _rightPlayerBulletSpeed = slowdownBulletSpeed;
        }
        else
        {
            leftPlayerIsSlowedDown = true;
            leftPlayerSlowDownTimer = slowdownPickupTime;
            _leftPlayerBulletSpeed = slowdownBulletSpeed;
        }
    }
    public void TriggerStorm()
    {
        stormTriggered = true;
        stormTimer = 0;
    }
    public void ShieldUp(GameObject player)
    {
        if (player.gameObject.tag == "RightPlayer")
        {
            Instantiate(shieldPrefab).GetComponent<ShieldController>().targetLeftPlayer = false;
        }
        else
        {
            Instantiate(shieldPrefab);
        }
    }
    #endregion

    // Private methods
    #region Private methods
    IEnumerator CheckForPickupSpawn() // Generates a random number and if said number is smaller than chanceForPickupToSpawnPerSecond, spawns a random pickup.
    {
        decidingWhetherToSpawnPickup = true;

        yield return new WaitForSeconds(1);

        int randomNumber = Random.Range(0, 100);
        if (randomNumber <= chanceForPickupToSpawnPerSecond)
        {
            int randomPickup = Random.Range(0, 5);

            switch (randomPickup)
            {
                case 0:
                    {
                        Instantiate(lifePickupPrefab, new Vector3(0, 6, 0), new Quaternion());
                    }
                    break;
                case 1:
                    {
                        Instantiate(shieldPickupPrefab, new Vector3(0, 6, 0), new Quaternion());
                    }
                    break;
                case 2:
                    {
                        Instantiate(speedupPickupPrefab, new Vector3(0, 6, 0), new Quaternion());
                    }
                    break;
                case 3:
                    {
                        Instantiate(slowdownPickupPrefab, new Vector3(0, 6, 0), new Quaternion());
                    }
                    break;
                case 4:
                    {
                        Instantiate(stormPickupPrefab, new Vector3(0, 6, 0), new Quaternion());
                    }
                    break;
            }
            pickupSpawnCooldown = defaultPickupSpawnCooldown;
        }

        decidingWhetherToSpawnPickup = false;
    }
    IEnumerator CheckForLightningBoltSpawn() // Generates a random number and if said number is smaller than chanceForLightningBoltToSpawnPerSecond, spawns a lightning bolt at a random X location.
    {
        yield return new WaitForSeconds(1);

        int randomNumber = Random.Range(0, 100);
        if (randomNumber < chanceForLightningBoltToSpawnPerSecond)
        {
            float randomPosition = Random.Range((-gameViewHorizontalDistanceInMeters / 2.0f) + 1f, (gameViewHorizontalDistanceInMeters / 2.0f) - 1f);
            Instantiate(lightningBoltPrefab, new Vector3(randomPosition, 0, 0), new Quaternion());
        }
    }
    void OnScoreLoaded(Scene scene, LoadSceneMode mode) // Displays the winner when the "Score" scene is loaded.
    {
        if (scene.name == "Score")
        {
            TMPro.TMP_Text winnerText = GameObject.FindGameObjectWithTag("WinnerText").GetComponent<TMPro.TMP_Text>();
            spawnPickups = false;

            if (loser == leftPlayer)
            {
                winnerText.text = "Player 2!";
            }
            else
            {
                winnerText.text = "Player 1!";
            }
        }
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _leftPlayer = GameObject.FindGameObjectWithTag("LeftPlayer");
        _rightPlayer = GameObject.FindGameObjectWithTag("RightPlayer");
        _leftPlayerBulletSpeed = defaultBulletSpeed;
        _rightPlayerBulletSpeed = defaultBulletSpeed;
        leftPlayerSpeedupTimer = speedupPickupTime;
        rightPlayerSpeedupTimer = speedupPickupTime;

        SceneManager.sceneLoaded += OnScoreLoaded;
    }

    private void FixedUpdate()
    {
        // Handle pickup spawning.
        if (spawnPickups)
        {
            if (pickupSpawnCooldown > 0) // Decrement cooldown
            {
                pickupSpawnCooldown -= Time.fixedDeltaTime;
            }
            else // Run a CheckForPickupSpawn()
            {
                if (!decidingWhetherToSpawnPickup)
                {
                    StartCoroutine(CheckForPickupSpawn());
                }
            }
        }

        // Handle the storm mechanic.
        if (stormTriggered)
        {
            stormTimer += Time.fixedDeltaTime;
            if (stormTimer > stormDuration) // Disable storm if the timer has ran out.
            {
                stormTriggered = false;
                stormTimer = 0;
            }
            else // Run a CheckForLightningBoltSpawn()
            {
                StartCoroutine(CheckForLightningBoltSpawn());
            }
        }

        // Handle bullet speedup mechanic.
        if (leftPlayerIsSpeedup)
        {
            leftPlayerSpeedupTimer -= Time.fixedDeltaTime;
            if (leftPlayerSpeedupTimer < 0) // Reset the bullet speed if timer has ran out.
            {
                _leftPlayerBulletSpeed = defaultBulletSpeed;
                leftPlayerIsSpeedup = false;
            }
        }
        if (rightPlayerIsSpeedup)
        {
            rightPlayerSpeedupTimer -= Time.fixedDeltaTime;
            if (rightPlayerSpeedupTimer < 0)
            {
                _rightPlayerBulletSpeed = defaultBulletSpeed;
                rightPlayerIsSpeedup = false;
            }
        }

        // Handle bullet slowdown mechanic.
        if (leftPlayerIsSlowedDown)
        {
            leftPlayerSlowDownTimer -= Time.fixedDeltaTime;
            if (leftPlayerSlowDownTimer < 0) // Reset the bullet speed if timer has ran out.
            {
                _leftPlayerBulletSpeed = defaultBulletSpeed;
                leftPlayerIsSlowedDown = false;
            }
        }
        if (rightPlayerIsSlowedDown)
        {
            rightPlayerSlowDownTimer -= Time.fixedDeltaTime;
            if (rightPlayerSlowDownTimer < 0)
            {
                _rightPlayerBulletSpeed = defaultBulletSpeed;
                rightPlayerIsSlowedDown = false;
            }
        }
    }
    #endregion
}
