using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    // Attributes
    #region Attributes
    // Setup variables
    public PlayerManager _playerManager;

    // References
    TMP_Text _player_livesText = null;
    Image[] _player_healthbars = null;
    int currentHealthBar;
    #endregion

    // Public properties
    #region MyRegion
    public TMP_Text Player_livesText => _player_livesText;
    public Image[] Player_healthbars => _player_healthbars;
    public PlayerManager PlayerManager
    {
        get
        {
            return _playerManager;
        }
        set
        {
            if (_playerManager == null)
            {
                _playerManager = value;
            }
            else
            {
                Debug.LogWarning("Attempting to modify PlayerManager reference after it has been set.");
            }
        }
    }
    #endregion

    // Public methods
    #region Public methods
    public void Init()
    {
        HealthAndLivesUIController controller = GameObject.FindGameObjectWithTag("HPandLivesUI").GetComponent<HealthAndLivesUIController>();
        if (PlayerManager.IsLeftPlayer)
        {
            _player_livesText = controller.Player1_livesText;
            _player_healthbars = controller.Player1_healthbars;
        }
        else
        {
            _player_livesText = controller.Player2_livesText;
            _player_healthbars = controller.Player2_healthbars;
        }
        currentHealthBar = 10;
    }
    public void UpdateLives()
    {
        _player_livesText.text = PlayerManager.Lives.ToString();
    }
    public void UpdateHealth()
    {
        _player_healthbars[currentHealthBar].fillAmount = PlayerManager.Health;
    }
    #endregion

}
