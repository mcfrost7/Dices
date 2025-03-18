using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControllerGlobalMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel_menu;
    [SerializeField] private GameObject button_continue;
    [SerializeField] private GameObject button_settings;
    [SerializeField] private GameObject button_exit_to_menu;
    [SerializeField] private GameObject button_exit_from_game;

    [SerializeField] private GameObject button_call_menu;
    [SerializeField] private GameObject button_call_task;
    [SerializeField] private GameObject image_freeze;

    public GameObject Image_freeze { get => image_freeze; set => image_freeze = value; }

    private void Awake()
    {
        panel_menu.SetActive(false);
        Image_freeze.SetActive(false);
    }

    public void CallMenuButton()
    {
        if (button_call_menu != null)
        {
            panel_menu.SetActive(true);
            Image_freeze.SetActive(true);
            Time.timeScale = 0;
        }

    }

    public void ContinueButton()
    {
        if (button_continue != null)
        {
            panel_menu.SetActive(false);
            Image_freeze.SetActive(false);
            Time.timeScale = 1;
        }
    }
    public void ExitToMenuButton()
    {
        ContinueButton();
        GameManager.Instance.gameObject.SetActive(false);
    }

    public void ExitFromGameButton()
    {
        Application.Quit();
        Debug.Log("Игра завершена.");
    }

    public void SettingsButton()
    {
        MenuManager menuManager = GetComponent<MenuManager>();
        if (menuManager != null)
        {
            menuManager.SettingsManager.SetActive(true);

        }
    }
}
