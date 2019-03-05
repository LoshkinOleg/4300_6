using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthAndLivesUIController : MonoBehaviour
{
    [SerializeField] Image[] _player1_healthbars = new Image[10];
    [SerializeField] Image[] _player2_healthbars = new Image[10];
    public Image[] Player1_healthbars => _player1_healthbars;
    public Image[] Player2_healthbars => _player2_healthbars;
}
