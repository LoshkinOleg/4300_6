using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuGamepadController : MonoBehaviour
{
    [SerializeField] EventSystem mainMenu_eventSystem = null;
    [SerializeField] GameObject[] mainMenuButtons = new GameObject[4];
    [SerializeField] GameObject creditsBack_button = null;
    [SerializeField] GameObject howToPlay_play_button = null;

    int mainMenuSelectedButton = 2;
    public static bool creditsAreOpen;
    public static bool howToPlayIsOpen;

    private void Update()
    {
        if (!creditsAreOpen && !howToPlayIsOpen) // If we're in main menu.
        {
            // If we're selecting a button.
            if (InputManager.ActiveDevice.LeftStick.Up.WasPressed || InputManager.ActiveDevice.RightStick.Up.WasPressed || InputManager.ActiveDevice.DPadUp.WasPressed)
            {
                mainMenuSelectedButton = mainMenuSelectedButton - 1 <= 0 ? 0 : mainMenuSelectedButton - 1;
            }
            else if (InputManager.ActiveDevice.LeftStick.Down.WasPressed || InputManager.ActiveDevice.RightStick.Down.WasPressed || InputManager.ActiveDevice.DPadDown.WasPressed)
            {
                mainMenuSelectedButton = mainMenuSelectedButton + 1 >= 3 ? 3 : mainMenuSelectedButton + 1;
            }
            mainMenu_eventSystem.SetSelectedGameObject(mainMenuButtons[mainMenuSelectedButton]);
        }
        else if (creditsAreOpen)
        {
            mainMenu_eventSystem.SetSelectedGameObject(creditsBack_button);
        }
        else if (howToPlayIsOpen)
        {
            mainMenu_eventSystem.SetSelectedGameObject(howToPlay_play_button);
        }

        // If there was a submit input.
        if (InputManager.ActiveDevice.Action1.WasPressed || InputManager.ActiveDevice.Action2.WasPressed || InputManager.ActiveDevice.Action3.WasPressed || InputManager.ActiveDevice.Action4.WasPressed)
        {
            if (!creditsAreOpen && !howToPlayIsOpen) // If we're in main menu.
            {
                switch (mainMenuSelectedButton)
                {
                    case 1:
                        howToPlayIsOpen = true;
                        break;
                    case 2:
                        creditsAreOpen = true;
                        break;
                }
            }
            else if (creditsAreOpen) creditsAreOpen = false;
            else if (howToPlayIsOpen) howToPlayIsOpen = false;

            if (mainMenuSelectedButton == 0 && !creditsAreOpen && !howToPlayIsOpen)
            {
                Toggle toggle = mainMenu_eventSystem.currentSelectedGameObject.GetComponent<Toggle>();
                toggle.isOn = !toggle.isOn;
            }
            else
            {
                mainMenu_eventSystem.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
            }
        }
    }
}
