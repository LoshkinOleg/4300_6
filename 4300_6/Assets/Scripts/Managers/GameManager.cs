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

    // References
    [SerializeField] GameObject pickupAndCrateManagers = null;
    [SerializeField] GameObject soundManager = null;
    static GameManager _instance = null;
    Player1 _player1 = null;
    Player2 _player2 = null;
    Rigidbody2D _player1RB = null;
    Rigidbody2D _player2RB = null;

    // Public properties
    public static GameManager instance => _instance;
    public float defaultBulletSpeed => _defaultBulletSpeed;
    public Player1 player1 => _player1;
    public Player2 player2 => _player2;
    public Rigidbody2D player1RB => _player1RB;
    public Rigidbody2D player2RB => _player2RB;
    public float gameViewHorizontalDistanceInMeters => _gameViewHorizontalDistanceInMeters;
    public float gameViewVerticalDistanceInMeters => _gameViewVerticalDistanceInMeters;
    public Vector3 averagePlayerPosition => _averagePlayerPosition;

    // Private variables
    GameObject loser = null;
    float _gameViewHorizontalDistanceInMeters = 17.78f;
    float _gameViewVerticalDistanceInMeters = 10f;
    Vector3 _averagePlayerPosition = new Vector3();
    #endregion

    // Public methods
    #region Public methods
    public void GameOver(GameObject loser)
    {
        this.loser = loser;
        SceneManager.LoadScene("Score");
    }
    #endregion

    // Private methods
    #region Private methods
    void OnScoreLoaded(Scene scene, LoadSceneMode mode) // Displays the winner when the "Score" scene is loaded.
    {
        if (scene.name == "Score")
        {
            TMPro.TMP_Text winnerText = GameObject.FindGameObjectWithTag("WinnerText").GetComponent<TMPro.TMP_Text>();

            if (loser == player1)
            {
                winnerText.text = "Player 2!";
            }
            else
            {
                winnerText.text = "Player 1!";
            }
        }
        else if(scene.name == "Level1")
        {
            if (PickupManager.instance == null && CrateManager.instance == null)
            {
                Instantiate(pickupAndCrateManagers);
            }
            else if ((PickupManager.instance != null && CrateManager.instance == null) || (PickupManager.instance == null && CrateManager.instance != null))
            {
                Debug.LogError("GameManager.cs: Reference to one of the managers is missing: PickupManager: " + PickupManager.instance + " ; CrateManager: " + CrateManager.instance);
            }

            _player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player1>();
            _player1RB = _player1.gameObject.GetComponent<Rigidbody2D>();
            _player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<Player2>();
            _player2RB = _player2.gameObject.GetComponent<Rigidbody2D>();
        }
        if (scene.name == "MainMenu")
        {
            if (SoundManager.instance == null)
            {
                Instantiate(soundManager);
            }
        }
    }
    Vector3 FindAveragePlayerPosition()
    {
        // Reset targetPosition
        Vector3 averagePosition = new Vector3();

        // Find average position
        averagePosition += player1.transform.position;
        averagePosition += player2.transform.position;
        averagePosition /= 2;

        return averagePosition;
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnScoreLoaded;
    }
    private void FixedUpdate()
    {
        _averagePlayerPosition = FindAveragePlayerPosition();
    }
    #endregion
}
