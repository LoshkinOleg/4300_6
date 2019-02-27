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
    [HideInInspector] public PlayerManager _playerManager;

    // Inspector variables
    [SerializeField] float uiHighlightLifetime = 0.3f;

    // References
    [SerializeField] Sprite[] healthbar_Sprites = new Sprite[2]; // 0: red bar, 1: yellow bar
    [SerializeField] Sprite[] life_Sprites = new Sprite[2]; // 0: red heart, 1: yellow heart
    TMP_Text _player_livesText = null;
    SpriteRenderer player_livesSpriteRenderer = null;
    Image[] _player_healthbars = null;

    // Private variables
    int _currentHealthBar;
    #endregion

    // Public properties
    #region Public properties
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
    public int CurrentHealthBar
    {
        get { return _currentHealthBar; }
        set { _currentHealthBar = value; }
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
            player_livesSpriteRenderer = controller.Player1_livesSpriteRenderer;
        }
        else
        {
            _player_livesText = controller.Player2_livesText;
            _player_healthbars = controller.Player2_healthbars;
            player_livesSpriteRenderer = controller.Player2_livesSpriteRenderer;
        }
        _currentHealthBar = 9;
    }
    public void UpdateLives()
    {
        _player_livesText.text = PlayerManager.Lives.ToString();
    }
    public void UpdateHealth()
    {
        _player_healthbars[_currentHealthBar].fillAmount = PlayerManager.Health;
    }
    public void HighlightHealthBar()
    {
        _player_healthbars[_currentHealthBar].sprite = healthbar_Sprites[1];
        StartCoroutine(ResetHealthBarSprite());
    }
    public void HighlightLives()
    {
        player_livesSpriteRenderer.sprite = life_Sprites[1];
        StartCoroutine(ResetLifeSprite());
    }
    #endregion

    // Private methods
    #region Private methods
    IEnumerator ResetHealthBarSprite()
    {
        yield return new WaitForSeconds(uiHighlightLifetime);
        _player_healthbars[_currentHealthBar].sprite = healthbar_Sprites[0];
    }
    IEnumerator ResetLifeSprite()
    {
        yield return new WaitForSeconds(uiHighlightLifetime);
        player_livesSpriteRenderer.sprite = life_Sprites[0];
    }
    #endregion

}
