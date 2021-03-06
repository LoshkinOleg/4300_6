﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] EventSystem level1_eventSystem = null;
    [SerializeField] GameObject[] buttons = new GameObject[2];
    int selectedButton;

    private void Update()
    {
        if (GameManager.Instance.IsPaused)
        {
            if (InputManager.ActiveDevice.LeftStick.Up.WasPressed || InputManager.ActiveDevice.RightStick.Up.WasPressed || InputManager.ActiveDevice.DPadUp.WasPressed || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                selectedButton = selectedButton - 1 <= 0 ? 0 : selectedButton - 1;
            }
            else if (InputManager.ActiveDevice.LeftStick.Down.WasPressed || InputManager.ActiveDevice.RightStick.Down.WasPressed || InputManager.ActiveDevice.DPadDown.WasPressed || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                selectedButton = selectedButton + 1 >= 1 ? 1 : selectedButton + 1;
            }
            level1_eventSystem.SetSelectedGameObject(buttons[selectedButton]);

            if (InputManager.ActiveDevice.AnyButtonWasPressed || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                level1_eventSystem.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
            }
        }

        if (InputManager.ActiveDevice.CommandWasPressed)
        {
            GameManager.Instance.TogglePause();
        }
    }
}
