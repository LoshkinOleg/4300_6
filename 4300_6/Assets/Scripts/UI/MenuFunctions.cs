using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
    // References
    [SerializeField] GameObject mainMenuPanel = null;
    [SerializeField] GameObject creditsMenuPanel = null;
    [SerializeField] GameObject howToPlayPanel = null;

    public void Play()
    {
        SceneManager.LoadScene("Level1");
    }

    public void DisplayHowToPlay()
    {
        if (howToPlayPanel != null && mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
            howToPlayPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("MenuFunctions.cs: Trying to interact with a non existant panel.");
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL && !UNITY_EDITOR
        Application.OpenURL("about:blank");
#else
        Application.Quit();
#endif
    }

    public void TogglePanels()
    {
        if (mainMenuPanel != null && creditsMenuPanel != null)
        {
            mainMenuPanel.SetActive(!mainMenuPanel.activeSelf);
            creditsMenuPanel.SetActive(!creditsMenuPanel.activeSelf);
        }
        else
        {
            Debug.LogError("MenuFunctions.cs: Trying to interact with a non existant panel.");
        }
    }

    public void GoToMainMenu()
    {
        Destroy(GameObject.FindGameObjectWithTag("GameController"));
        SceneManager.LoadScene("MainMenu");
    }
}
