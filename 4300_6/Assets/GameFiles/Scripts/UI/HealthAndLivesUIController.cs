using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthAndLivesUIController : MonoBehaviour
{
    [SerializeField] TMP_Text _player1_livesText = null;
    [SerializeField] TMP_Text _player2_livesText = null;
    [SerializeField] Image[] _player1_healthbars = new Image[10];
    [SerializeField] Image[] _player2_healthbars = new Image[10];
    public TMP_Text Player1_livesText => _player1_livesText;
    public TMP_Text Player2_livesText => _player2_livesText;
    public Image[] Player1_healthbars => _player1_healthbars;
    public Image[] Player2_healthbars => _player2_healthbars;
}
