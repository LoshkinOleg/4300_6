using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

// /!\ Is setup in OnSceneLoad, not in Start()! /!\

public class GameManager : MonoBehaviour
{

    // Attributes
    #region Attributes
    // References
    [SerializeField] GameObject pickupAndCrateManagers = null;
    [SerializeField] GameObject soundManager = null;
    BoxCollider2D _bottomBoundsCollider = null;
    static GameManager _instance = null;
    PlayerManager _player1 = null;
    PlayerManager _player2 = null;
    Rigidbody2D _player1RB = null;
    Rigidbody2D _player2RB = null;
    FeedbacksUIController _feedbackUIController;

    // Public properties
    public static GameManager Instance => _instance;
    public PlayerManager Player1 => _player1;
    public PlayerManager Player2 => _player2;
    public Rigidbody2D Player1RB => _player1RB;
    public Rigidbody2D Player2RB => _player2RB;
    public float GameViewHorizontalDistanceInMeters => _gameViewHorizontalDistanceInMeters;
    public float GameViewVerticalDistanceInMeters => _gameViewVerticalDistanceInMeters;
    public Vector3 AveragePlayerPosition => _averagePlayerPosition;
    public BoxCollider2D BottomBoundsCollider => _bottomBoundsCollider;
    public FeedbacksUIController feedbackUIController => _feedbackUIController;

    // Private variables
    GameObject loser = null;
    const float _gameViewHorizontalDistanceInMeters = 17.78f;
    const float _gameViewVerticalDistanceInMeters = 10f;
    Vector3 _averagePlayerPosition = new Vector3();
    List<InputDevice> devicesBeingUsed = new List<InputDevice>();
    #endregion

    // Public methods
    #region Public methods
    public void GameOver(GameObject loser)
    {
        this.loser = loser;
        SceneManager.LoadScene("Score");
    }
    public float CalculateNorm(Vector2 vector)
    {
        return Mathf.Sqrt(Mathf.Pow(vector.x, 2) + Mathf.Pow(vector.y, 2));
    }
    #endregion

    // Private methods
    #region Private methods
    void OnScoreLoaded(Scene scene, LoadSceneMode mode) // Displays the winner when the "Score" scene is loaded.
    {
        if (scene.name == "Score")
        {
            TMPro.TMP_Text winnerText = GameObject.FindGameObjectWithTag("WinnerText").GetComponent<TMPro.TMP_Text>();

            if (loser == Player1)
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

            _player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<PlayerManager>();
            _player1RB = _player1.gameObject.GetComponent<Rigidbody2D>();
            _player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerManager>();
            _player2RB = _player2.gameObject.GetComponent<Rigidbody2D>();
            _bottomBoundsCollider = GameObject.FindGameObjectWithTag("BottomBounds").GetComponent<BoxCollider2D>();
            _feedbackUIController = GameObject.FindGameObjectWithTag("FeedbackUI").GetComponent<FeedbacksUIController>();
        }
        else if (scene.name == "MainMenu")
        {
            if (SoundManager.Instance == null)
            {
                Instantiate(soundManager);
            }
        }
    }
    void AssignADevice(InputDevice device)
    {
        if (Player1.gamepad == null)
        {
            if (DeviceIsNotTaken(device))
            {
                Player1.gamepad = device;
                devicesBeingUsed.Add(device);
            }
        }
        else if (Player2.gamepad == null)
        {
            if (DeviceIsNotTaken(device))
            {
                Player2.gamepad = device;
                devicesBeingUsed.Add(device);
            }
        }
    }
    bool DeviceIsNotTaken(InputDevice device)
    {
        foreach (var item in devicesBeingUsed)
        {
            if (device == item)
            {
                return false;
            }
        }
        return true;
    }
    #endregion

    // Inherited methods
    #region Inherited methods
    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnScoreLoaded;
        if (SoundManager.Instance == null)
        {
            Instantiate(soundManager);
        }
    }

    private void Start()
    {
        InputManager.OnActiveDeviceChanged += AssignADevice; // Putting this in Start should hopefully avoid errors.
    }
    #endregion
}
