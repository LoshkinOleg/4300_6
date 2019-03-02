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
    [SerializeField] Sprite[] killstreak_Sprites = new Sprite[5]; // 0: blank sprite, 1: private, 2: corp, 3: sergent, 4: commander
    SpriteRenderer killstreak_SpriteRenderer = null;
    Image[] healthbars_Images = null;

    // Private variables
    int killstreak;
    #endregion

    // Public properties
    #region Public properties
    public Image[] Player_healthbars_Images => healthbars_Images;
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
            healthbars_Images = controller.Player1_healthbars;
            killstreak_SpriteRenderer = controller.Player1_killstreak_SpriteRenderer;
        }
        else
        {
            healthbars_Images = controller.Player2_healthbars;
            killstreak_SpriteRenderer = controller.Player2_killstreak_SpriteRenderer;
        }
    }
    public void HighlightHealthBar(int damage)
    {
        StartCoroutine(DisplayHealthbarHit(damage));
    }
    public void IncrementKillstreak()
    {
        killstreak++;
        if (killstreak > 4)
        {
            killstreak = 4;
        }
        killstreak_SpriteRenderer.sprite = killstreak_Sprites[killstreak];
    }
    public void ResetKillstreak()
    {
        killstreak_SpriteRenderer.sprite = killstreak_Sprites[0];
    }
    public void ResetHealthbar()
    {
        foreach (var item in healthbars_Images)
        {
            StopAllCoroutines();
            item.sprite = healthbar_Sprites[0];
            item.fillAmount = 1;
        }
    }
    #endregion

    // Private methods
    #region Private methods
    IEnumerator DisplayHealthbarHit(int damage)
    {
        int startingHealth = PlayerManager.Health;
        int resultingHealth = PlayerManager.Health + damage;

        // Set sprites to yellow.
        if (damage > 0) // Healing player.
        {
            for (int i = 0; i < Mathf.Abs(damage); i++)
            {
                if (startingHealth - 1 + i >= 0 && startingHealth - 1 + i < 10)
                {
                    Player_healthbars_Images[startingHealth + i].fillAmount = 1;
                    Player_healthbars_Images[startingHealth + i].sprite = healthbar_Sprites[1];
                }
            }
        }
        else if (damage < 0) // Damaging player.
        {
            for (int i = 0; i < Mathf.Abs(damage); i++)
            {
                if (startingHealth - 1 - i >= 0 && startingHealth - 1 - i < 10)
                {
                    Player_healthbars_Images[startingHealth - 1 - i].sprite = healthbar_Sprites[1];
                }
            }
        }

        yield return new WaitForSeconds(uiHighlightLifetime);

        // Set sprites to red and fill to 0.

        if (damage > 0) // Healing player
        {
            for (int i = 0; i < Mathf.Abs(damage); i++)
            {
                if (resultingHealth - 1 - i >= 0 && resultingHealth - 1 - i < 10)
                {
                    Player_healthbars_Images[resultingHealth - 1 - i].sprite = healthbar_Sprites[0];
                }
            }
        }
        else if (damage < 0) // Damaging player
        {
            for (int i = 0; i < Mathf.Abs(damage); i++)
            {
                if (resultingHealth + i >= 0 && resultingHealth + i < 10)
                {
                    Player_healthbars_Images[resultingHealth + i].sprite = healthbar_Sprites[0];
                    Player_healthbars_Images[resultingHealth + i].fillAmount = 0;    
                }
            }
        }
    }
    #endregion
}
