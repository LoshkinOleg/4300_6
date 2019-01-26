using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Inspector variables
    [SerializeField] float _defaultBulletSpeed = 10;
    [SerializeField] float defaultPickupSpawnCooldown = 4;
    [SerializeField] float chanceForPickupToSpawnPerSecond = 10;
    [SerializeField] float _healthPickupValue = 0.5f;
    [SerializeField] float speedupPickupValue = 20;
    [SerializeField] float speedupPickupTime = 5;
    [SerializeField] float stormDuration = 7;
    [SerializeField] float gameViewHorizontalDistanceInMeters = 17.78f;
    [SerializeField] float chanceForLightningBoltToSpawnPerSecond = 3;

    // References
    [SerializeField] GameObject lifePickupPrefab = null;
    [SerializeField] GameObject speedPickupPrefab = null;
    [SerializeField] GameObject stormPickupPrefab = null;
    [SerializeField] GameObject lightningBoltPrefab = null;

    // Public properties
    public static GameManager instance => _instance;
    public float defaultBulletSpeed => _defaultBulletSpeed;
    public GameObject leftPlayer => _leftPlayer;
    public GameObject rightPlayer => _rightPlayer;
    public float healthPickupValue => _healthPickupValue;
    public float leftPlayerBulletSpeed => _leftPlayerBulletSpeed;
    public float rightPlayerBulletSpeed => _rightPlayerBulletSpeed;

    // Private variables and references
    static GameManager _instance = null;
    GameObject _leftPlayer = null;
    GameObject _rightPlayer = null;
    float _leftPlayerBulletSpeed;
    float _rightPlayerBulletSpeed;
    float pickupSpawnCooldown = 0;
    bool decidingWhetherToSpawnPickup = false;
    bool stormTriggered = false;
    float stormTimer = 0;
    bool leftPlayerIsSpeedup = false;
    bool rightPlayerIsSpeedup = false;
    float leftPlayerSpeedupTimer = 0;
    float rightPlayerSpeedupTimer = 0;
    GameObject loser = null;
    bool spawnPickups = true;
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
            _leftPlayerBulletSpeed = speedupPickupValue;
        }
        else
        {
            rightPlayerIsSpeedup = true;
            rightPlayerSpeedupTimer = speedupPickupTime;
            _rightPlayerBulletSpeed = speedupPickupValue;
        }
    }
    public void TriggerStorm()
    {
        stormTriggered = true;
        stormTimer = 0;
    }
    #endregion

    // Private methods
    #region Private methods
    IEnumerator CheckForPickupSpawn()
    {

        decidingWhetherToSpawnPickup = true;

        yield return new WaitForSeconds(1);

        int randomNumber = Random.Range(0, 100);
        if (randomNumber <= chanceForPickupToSpawnPerSecond)
        {
            int randomPickup = Random.Range(0, 3);
            if (randomPickup == 0)
            {
                Instantiate(lifePickupPrefab, new Vector3(0, 6, 0), new Quaternion());
            }
            else if(randomPickup == 1)
            {
                Instantiate(speedPickupPrefab, new Vector3(0, 6, 0), new Quaternion());
            }
            else
            {
                Instantiate(stormPickupPrefab, new Vector3(0, 6, 0), new Quaternion());
            }
            pickupSpawnCooldown = defaultPickupSpawnCooldown;
        }

        decidingWhetherToSpawnPickup = false;
    }
    IEnumerator CheckForLightningBoltSpawn()
    {
        yield return new WaitForSeconds(1);

        int randomNumber = Random.Range(0, 100);
        if (randomNumber < chanceForLightningBoltToSpawnPerSecond)
        {
            float randomPosition = Random.Range((-gameViewHorizontalDistanceInMeters / 2.0f) + 1f, (gameViewHorizontalDistanceInMeters / 2.0f) - 1f);
            Instantiate(lightningBoltPrefab, new Vector3(randomPosition, 0, 0), new Quaternion());
        }
    }
    void OnScoreLoaded(Scene scene, LoadSceneMode mode)
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
        if (spawnPickups)
        {
            if (pickupSpawnCooldown > 0)
            {
                pickupSpawnCooldown -= Time.fixedDeltaTime;
            }
            else
            {
                if (!decidingWhetherToSpawnPickup)
                {
                    StartCoroutine(CheckForPickupSpawn());
                }
            }
        }

        if (stormTriggered)
        {
            stormTimer += Time.fixedDeltaTime;
            if (stormTimer > stormDuration)
            {
                stormTriggered = false;
                stormTimer = 0;
            }
            else
            {
                StartCoroutine(CheckForLightningBoltSpawn());
            }
        }

        if (leftPlayerIsSpeedup)
        {
            leftPlayerSpeedupTimer -= Time.fixedDeltaTime;
            if (leftPlayerSpeedupTimer < 0)
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
    }
    #endregion
}
