using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;
using UnityEngine.EventSystems;

// /!\ Is setup in OnSceneChange(), not in Start()! /!\

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
    GameObject pausePanel = null;

    // Prefabs
    [SerializeField] GameObject level1Managers = null;
    [SerializeField] GameObject soundManager = null;

    // Private variables
    const float _GAME_VIEW_HORIZONTAL_DISTANCE_IN_METERS = 17.78f;
    const float _GAME_VIEW_VERTICAL_DISTANCE_IN_METERS = 10f;
    string currentScene;
    List<InputDevice> devicesBeingUsed = new List<InputDevice>();
    bool _isPaused;
    #endregion

    // Public properties
    #region Public properties
    public static GameManager Instance => _instance;
    public PlayerManager[] Players => _players;
    public BoxCollider2D BottomBoundsCollider => _bottomBoundsCollider;
    public ScreenShake ScreenShake => _screenShake;
    public GameObject FloatingTextFeedbackUI => _floatingTextFeedbackUI;
    public float GameViewHorizontalDistanceInMeters => _GAME_VIEW_HORIZONTAL_DISTANCE_IN_METERS;
    public float GameViewVerticalDistanceInMeters => _GAME_VIEW_VERTICAL_DISTANCE_IN_METERS;
    public bool IsPaused => _isPaused;
    #endregion

    // Public methods
    #region Public methods
    public void TogglePause()
    {
        Time.timeScale = Time.timeScale == 1 ? 0 : 1;
        pausePanel.SetActive(!pausePanel.activeSelf);
        _isPaused = !_isPaused;
        foreach (var item in Players)
        {
            item.ResetInputs();
        }
    }
    #endregion

    // Private methods
    #region Private methods
    void OnSceneChange(Scene scene, LoadSceneMode mode) // Sets up GM depending on scene we're in.
    {
        currentScene = scene.name;

        switch (currentScene)
        {
            case "MainMenu":
                {
                    if (SoundManager.Instance == null)
                    {
                        Instantiate(soundManager);
                    }

                    // Check if we've successfully set up.
                    if (SoundManager.Instance   == null)
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
                    pausePanel = GameObject.FindGameObjectWithTag("PausePanel");

                    // Check if we've successfully set up.
                    if (SoundManager.Instance       == null ||
                        PickupManager.Instance      == null ||
                        CrateManager.Instance       == null)
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
                        _floatingTextFeedbackUI == null ||
                        pausePanel              == null)
                    {
                        Debug.LogError("GM not set up properly!");
                    }

                    // Finish setting things up.
                    Time.timeScale = 1;
                    StartCoroutine(ResetGamepads());
                    pausePanel.SetActive(false);
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
        if (currentScene == "Level1")
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
    IEnumerator ResetGamepads()
    {
        yield return new WaitForEndOfFrame(); // Needed to let PlayerInputHandler.cs be set up before attempting to get it's attributes.
        foreach (var item in Players)
        {
            item.Gamepad = null;
        }
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
    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (currentScene)
            {
                case "MainMenu":
                    Application.Quit();
                    break;
                case "Level1":
                    TogglePause();
                    break;
                default:
                    Debug.LogError("Unknown scene name!" + SceneManager.GetActiveScene().name);
                    break;
            }
        }

        if (currentScene == "Level1")
        {
            if (Players[0].Gamepad == null) // Prevents the odd behaviour where you have to wait for OnActiveDeviceChanged to assign a controller to a player. This way, any active gamepad will be assigned to left player right away.
            {
                if (InputManager.ActiveDevice.AnyButtonWasPressed || InputManager.ActiveDevice.LeftStick.HasChanged || InputManager.ActiveDevice.RightStick.HasChanged || InputManager.ActiveDevice.DPad.HasChanged || InputManager.ActiveDevice.LeftBumper.HasChanged || InputManager.ActiveDevice.RightBumper.HasChanged)
                {
                    Players[0].Gamepad = InputManager.ActiveDevice;
                }
            }
        }
    }
    #endregion
}
