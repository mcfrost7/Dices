using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMNG : MonoBehaviour
{
    public void OnNewGameStart()
    {
        GameDataMNG.Instance.StartNewGame();
    }

    public void OnContinueGame()
    {
        GameDataMNG.Instance.LoadGame();
    }

    public void OnExitGame()
    {
        Application.Quit();
        Debug.Log("Игра завершена.");
    }

}
