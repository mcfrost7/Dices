using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuMNG : MonoBehaviour
{
    [Header("Menu UI")]
    [SerializeField] private GameObject _menuWindow;
    [SerializeField] private Button _menu;
    [SerializeField] private Button _continue;
    [SerializeField] private Button _settings;
    [SerializeField] private Button _exitMenu;
    [SerializeField] private Button _exitGame;


    [Header("Main Menu")]
    [SerializeField] private Button _continueMain;
    [SerializeField] private Button _playMain;
    [SerializeField] private Button _settingsMain;
    [SerializeField] private Button _exitGameMain;
    [Header("Help windows")]
    [SerializeField] private GameObject _InfoPanel;
    [SerializeField] private GameObject _freeze;

    [Header("Task panel")]
    [SerializeField] private GameObject _task;
    [SerializeField] private Button _taskButton;
    [Header("Down panel")]
    [SerializeField] private GameObject _downPanel;
    [SerializeField] private Button _teamButton;
    [SerializeField] private Button _teamBackButton;

    private GameObject _currentActiveWindow;
    private bool _taskVisibility = false;

    public static MenuMNG Instance { get; private set; }
    public Button ContinueMain { get => _continueMain; set => _continueMain = value; }

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
        SetupAllUI();
    }

    private void SetupAllUI()
    {
        SetupMenuButtons();
        SetupMainButtons();
        SetupTaskPanel();
        SetupDownPanel();
    }

    public void SetupMenuButtons()
    {
        _menu.onClick.AddListener(()=>ShowMenu(_menuWindow));
        _continue.onClick.AddListener(()=>HideWindow());
        _settings.onClick.AddListener(()=>SetupSettings());
        _exitMenu.onClick.AddListener(()=> SetupMainMenu());
        _exitGame.onClick.AddListener(()=> GameMNG.Instance.OnExitGame());
    }
    public void SetupMainButtons()
    {
        ContinueMain.onClick.AddListener(()=>GameMNG.Instance.OnContinueGame());
        _playMain.onClick.AddListener(() => GameMNG.Instance.OnNewGameStart());
        _settingsMain.onClick.AddListener(() => GlobalWindowController.Instance.ShowSettings());
        _exitGameMain.onClick.AddListener(() => GameMNG.Instance.OnExitGame());
    }

    public void SetupTaskPanel()
    {
        _taskButton.onClick.AddListener(()=>CallTask());
    }    
    public void SetupDownPanel()
    {
        _teamButton.onClick.AddListener(()=>GlobalWindowController.Instance.ShowTeam());
        _teamButton.onClick.AddListener(() => ChangeVisibilityOfDownPanel());
        _teamButton.onClick.AddListener(() => UnitsPanelUI.Instance.OnMenuLoad());
        _teamBackButton.onClick.AddListener(() => GlobalWindowController.Instance.ShowGlobalCanvas());
        _teamBackButton.onClick.AddListener(() => ChangeVisibilityOfDownPanel());
    }
    public void HideWindow()
    {
        CallFreezePanel(0);
        _currentActiveWindow.SetActive(false);
    }

    private void SetupSettings()
    {
        HideWindow();
        GlobalWindowController.Instance.ShowSettings();
    }    
    private void SetupMainMenu()
    {
        if (GameMNG.Instance.IsGameLoaded)
        {
            ContinueMain.interactable = true;
        }
        HideWindow();
        GlobalWindowController.Instance.ShowMenu();

    }

    public void ShowMenu(GameObject _window)
    {
        _currentActiveWindow = _window;
        _window.SetActive(true);
        CallFreezePanel(1);

    }

    public void CallFreezePanel(int _direction)
    {
        bool _boolDirection = _direction == 1 ? true : false;
        _freeze.SetActive(_boolDirection);
    }

    public void ShowInfo()
    {
        _currentActiveWindow = _InfoPanel;
        _freeze.SetActive(true);
    }

    public void CallTask()
    {
        if (_taskVisibility == false)
        {
            _task.transform.DOLocalMoveX(_task.transform.localPosition.x - 500, 0.8f).SetEase(Ease.InOutExpo);
        } else
        {
            _task.transform.DOLocalMoveX(_task.transform.localPosition.x + 500, 0.8f).SetEase(Ease.InOutExpo);
        }
        _taskVisibility = !_taskVisibility;
    }
    
    public void ChangeVisibilityOfDownPanel()
    {
        _downPanel.SetActive(!_downPanel.activeSelf);
    }

    public void TurnOnUI(GameObject _gameObject)
    {
        _gameObject.SetActive(true);
    }
    public void TurnOffUI(GameObject _gameObject)
    {
        _gameObject.SetActive(false);
    }
}
