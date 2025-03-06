using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMNG : MonoBehaviour
{
    public void OnNewGameStart()
    {
        MapMNG.Instance.StartNewGame();
    }

    public void OnContinueGame()
    {
        MapMNG.Instance.LoadGame();
    }

}
