using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public Canvas menuCanvas;
    public GameObject GameManager;
    public void CreateNewGame()
    {
        menuCanvas.gameObject.SetActive(false);
        GameManager gameManager = GameManager.GetComponent<GameManager>();
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
