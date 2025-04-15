using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLogic : MonoBehaviour
{
    // mainmenu button, setting button, exit button
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitButton;

    private void Awake() {
        startButton.onClick.AddListener(OnMainMenuButtonClick);
        settingButton.onClick.AddListener(OnSettingButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnMainMenuButtonClick() {
        // Load the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Selecting Level");
    }

    private void OnSettingButtonClick() {
        // Load the settings scene
        // UnityEngine.SceneManagement.SceneManager.LoadScene("Settings");
    }

    private void OnExitButtonClick() {
        // Exit the application
        Application.Quit();
    }

}
