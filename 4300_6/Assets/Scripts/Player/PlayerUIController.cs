using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    // Attributes
    #region Attributes
    // References
    [HideInInspector] public PlayerManager _playerManager = null;
    [SerializeField] GameObject healthImageGO = null;
    Image healthImage = null;

    // Public properties
    public PlayerManager playerManager
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
    public void UpdateHealthBar()
    {
        healthImage.fillAmount = playerManager.health;
    }
    public void Init()
    {
        healthImage = healthImageGO.GetComponent<Image>();
    }
    #endregion
}
