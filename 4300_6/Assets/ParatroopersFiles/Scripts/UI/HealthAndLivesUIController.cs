using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthAndLivesUIController : MonoBehaviour
{
    [SerializeField] Image[] _player1_healthbars = new Image[10];
    [SerializeField] Image[] _player2_healthbars = new Image[10];
    [SerializeField] SpriteRenderer _player1_KillstreakSpriteRenderer = null;
    [SerializeField] SpriteRenderer _player2_KillstreakSpriteRenderer = null;
    public Image[] Player1_healthbars => _player1_healthbars;
    public Image[] Player2_healthbars => _player2_healthbars;
    public SpriteRenderer Player1_killstreak_SpriteRenderer => _player1_KillstreakSpriteRenderer;
    public SpriteRenderer Player2_killstreak_SpriteRenderer => _player2_KillstreakSpriteRenderer;
}
