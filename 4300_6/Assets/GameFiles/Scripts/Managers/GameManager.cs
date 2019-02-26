using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

// /!\ Is setup in OnSceneChange, not in Start()! /!\
public class GameManager : MonoBehaviour
{
    // Attributes
    #region Attributes
    // References
    static GameManager _instance = null;
    BoxCollider2D _bottomBoundsCollider = null;
    PlayerManager[] _players = new PlayerManager[2];
    ScreenShake _screenShake = null;
    GameObject _floatingTextFeedbackUI = null;

    // Prefabs
    [SerializeField] GameObject level1Managers = null;
    [SerializeField] GameObject soundManager = null;

    // Public properties
    public static GameManager Instance => _instance;
    public PlayerManager[] Players => _players;
    public float GameViewHorizontalDistanceInMeters => _gameViewHorizontalDistanceInMeters;
    public float GameViewVerticalDistanceInMeters => _gameViewVerticalDistanceInMeters;
    public BoxCollider2D BottomBoundsCollider => _bottomBoundsCollider;
    public ScreenShake ScreenShake => _screenShake;
    public GameObject FloatingTextFeedbackUI => _floatingTextFeedbackUI;

    // Private variables
    GameObject loser = null;
    const float _gameViewHorizontalDistanceInMeters = 17.78f;
    const float _gameViewVerticalDistanceInMeters = 10f;
    List<InputDevice> devicesBeingUsed = new List<InputDevice>();
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

                    // Check if we've successfully set up.
                    if (SoundManager.Instance == null)
                    {
                        Debug.LogError("GM not set up properly!");
                    }
                }
                break;
            case "Level1":
                {
                    if (PickupManager.Instance == null)
                    {
                        Instantiate(level1Managers);
                    }
                    if (SoundManager.Instance == null)
                    {
                        Instantiate(soundManager);
                    }

                    _players[0] = GameObject.FindGameObjectWithTag("Player1").GetComponent<PlayerManager>();
                    _players[1] = GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerManager>();
                    _bottomBoundsCollider = GameObject.FindGameObjectWithTag("BottomBounds").GetComponent<BoxCollider2D>();
                    _screenShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<ScreenShake>();
                    _floatingTextFeedbackUI = GameObject.FindGameObjectWithTag("FloatingText_Canvas");

                    // Check if we've successfully set up.
                    if (SoundManager.Instance       == null ||
                        PickupManager.Instance      == null ||
                        CrateManager.Instance       == null ||
                        FeedbackManager.Instance    == null)
                    {
                        Debug.LogError("GM not set up properly!");
                    }
                    foreach (var item in _players)
                    {
                        if (item == null)
                        {
                            Debug.LogError("GM not set up properly!");
                        }
                    }
                    if (_bottomBoundsCollider   == null ||
                        _screenShake            == null ||
                        _floatingTextFeedbackUI == null)
                    {
                        Debug.LogError("GM not set up properly!");
                    }
                }
                break;
            case "Score":
                {
                    if (SoundManager.Instance == null)
                    {
                        Instantiate(soundManager);
                    }

                    TMPro.TMP_Text winnerText = GameObject.FindGameObjectWithTag("WinnerText").GetComponent<TMPro.TMP_Text>();

                    if (loser == Players[0])
                    {
                        winnerText.text = "Player 2!";
                    }
                    else
                    {
                        winnerText.text = "Player 1!";
                    }

                    // Check if we've successfully set up. If so, Stop all sound that might be playing on loop from the previous scene.
                    if (SoundManager.Instance == null)
                    {
                        Debug.LogError("GM not set up properly!");
                    }
                    else
                    {
                        SoundManager.Instance.StopAllSounds();
                    }
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
        if (Players[0].Gamepad == null)
        {
            if (DeviceIsNotTaken(device))
            {
                Players[0].Gamepad = device;
                devicesBeingUsed.Add(device);
            }
        }
        else if (Players[1].Gamepad == null)
        {
            if (DeviceIsNotTaken(device))
            {
                Players[1].Gamepad = device;
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
        InputManager.OnActiveDeviceChanged += AssignADevice;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    #endregion
}
