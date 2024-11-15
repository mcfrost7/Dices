using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private GameObject gameManager;
    [SerializeField] private GameObject settingsManager;



    public Canvas MenuCanvas { get => menuCanvas; set => menuCanvas = value; }
    public GameObject GameManager { get => gameManager; set => gameManager = value; }
    public GameObject SettingsManager { get => settingsManager; set => settingsManager = value; }



    private void Awake()
    {
        MenuCanvas.gameObject.SetActive(true); 
    }

    private void OnEnable()
    {
        MenuCanvas.gameObject.SetActive(true);
    }
    public void CreateNewGame()
    {
        MenuCanvas.gameObject.SetActive(false);
        GameManager gameManager = GameManager.GetComponent<GameManager>();
        gameManager.SetGameStatus(true);
        gameManager.EnableManager(GameManager, true);

    }

    public void ContinueGame()
    {
        MenuCanvas.gameObject.SetActive(false);
        GameManager gameManager = GameManager.GetComponent<GameManager>();
        gameManager.SetGameStatus(false);
        gameManager.EnableManager(GameManager, true);

    }

    public void CallSettings()
    {
        SettingsManager manager = settingsManager.GetComponent<SettingsManager>();
        manager.Manager_to_enable = this.gameObject;
        MenuCanvas.gameObject.SetActive(false);
        SettingsManager.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void ExitToMenu()
    {
        gameManager.SetActive(false);
        MenuCanvas.gameObject.SetActive(true);
    }


    public void Exit()
    {
        Application.Quit();
        Debug.Log("Игра завершена.");
    }



}
