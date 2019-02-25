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
    [SerializeField] GameObject level1Managers = null;
    [SerializeField] GameObject soundManager = null;
    BoxCollider2D _bottomBoundsCollider = null;
    static GameManager _instance = null;
    PlayerManager _player1 = null;
    PlayerManager _player2 = null;
    //Rigidbody2D _player1RB = null;
    //Rigidbody2D _player2RB = null;
    ScreenShake _screenShake = null;

    // Public properties
    public static GameManager Instance => _instance;
    public PlayerManager Player1 => _player1;
    public PlayerManager Player2 => _player2;
    //public Rigidbody2D Player1RB => _player1RB;
    //public Rigidbody2D Player2RB => _player2RB;
    public float GameViewHorizontalDistanceInMeters => _gameViewHorizontalDistanceInMeters;
    public float GameViewVerticalDistanceInMeters => _gameViewVerticalDistanceInMeters;
    public Vector3 AveragePlayerPosition => _averagePlayerPosition;
    public BoxCollider2D BottomBoundsCollider => _bottomBoundsCollider;
    public ScreenShake screenShake => _screenShake;

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
    void OnSceneChange(Scene scene, LoadSceneMode mode) // Sets up GM depending on scene we're in.
    {
        switch (scene.name)
        {
            case "MainMenu":
                {
                    if (SoundManager.Instance == null)
                    {
                        Instantiate(soundManager);
                    }
                }
                break;
            case "Level1":
                {
                    if (PickupManager.instance == null)
                    {
                        Instantiate(level1Managers);
                    }

                    _player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<PlayerManager>();
                    _player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerManager>();
                    _bottomBoundsCollider = GameObject.FindGameObjectWithTag("BottomBounds").GetComponent<BoxCollider2D>();
                    _screenShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<ScreenShake>();
                }
                break;
            case "Score":
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

                    if (SoundManager.Instance != null)          SoundManager.Instance.StopAllSounds();          else Debug.LogWarning("Variable not set!");
                }
                break;
            default:
                {
                    Debug.LogError("Unknown scene passed to OnSceneChange: " + scene.name);
                }
                break;
        }
    }
    void AssignADevice(InputDevice device)
    {
        if (Player1.Gamepad == null)
        {
            if (DeviceIsNotTaken(device))
            {
                Player1.Gamepad = device;
                devicesBeingUsed.Add(device);
            }
        }
        else if (Player2.Gamepad == null)
        {
            if (DeviceIsNotTaken(device))
            {
                Player2.Gamepad = device;
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
        SceneManager.sceneLoaded += OnSceneChange;
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
