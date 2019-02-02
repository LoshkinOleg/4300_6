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
    static GameManager _instance = null;
    Player1 _player1 = null;
    // GameObject _player2 = null;

    // Public properties
    public static GameManager instance => _instance;
    public float defaultBulletSpeed => _defaultBulletSpeed;
    public Player1 player1 => _player1;
    // public Player2 player2 => _player2;

    // Private variables
    GameObject loser = null;
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

    private void Start()
    {
        _player1 = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player1>();
        // _player2 = GameObject.FindGameObjectWithTag("Player2").GetComponent<Player2>();
    }
    #endregion
}
