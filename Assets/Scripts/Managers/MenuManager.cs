using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public Canvas menuCanvas;
    public GameObject GameManager;


    private void Awake()
    {
        menuCanvas.gameObject.SetActive(true); 
    }
    public void CreateNewGame()
    {
        menuCanvas.gameObject.SetActive(false);
        GameManager gameManager = GameManager.GetComponent<GameManager>();
        gameManager.SetGameStatus(true);
        gameManager.EnableManager(GameManager, true);

    }

    public void ContinueGame()
    {
        menuCanvas.gameObject.SetActive(false);
        GameManager gameManager = GameManager.GetComponent<GameManager>();
        gameManager.SetGameStatus(false);
        gameManager.EnableManager(GameManager, true);

    }

    public void CallSettings()
    {

    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("Игра завершена.");
    }
}
