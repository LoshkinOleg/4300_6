using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// To be put on an EventSystem!

public class ScoreMenuGamepadController : MonoBehaviour
{
    EventSystem eventSystem = null;

    private void Start()
    {
        eventSystem = GetComponent<EventSystem>();
    }

    private void Update()
    {
        if (InputManager.ActiveDevice.AnyButtonWasPressed || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            eventSystem.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
        }
    }
}
