using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMNG : MonoBehaviour
{
    public static GameMNG Instance { get; private set; }
    public bool IsGameLoaded { get => isGameLoaded; set => isGameLoaded = value; }

    private bool isGameLoaded = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        SetupLoadedGameUI();
    }

    public void SetupLoadedGameUI()
    {
        IsGameLoaded = GameDataMNG.Instance.LoadGame();
        if (IsGameLoaded)
        {
            MenuMNG.Instance.ContinueMain.interactable = true;
        }
        else
        {
            MenuMNG.Instance.ContinueMain.interactable = false;
        }
    }
    public void OnNewGameStart()
    {
        IsGameLoaded = true;
        GameDataMNG.Instance.StartNewGame();
        MenuMNG.Instance.ShowLocation();
        GlobalWindowController.Instance.ShowGlobalCanvas();

    }

    public void OnContinueGame()
    {
        if (GameDataMNG.Instance.LoadGame())
        {
            GlobalWindowController.Instance.ShowGlobalCanvas();
            MenuMNG.Instance.ShowLocation();
        }
    }

    public void OnExitGame()
    {
        Application.Quit();
    }
}