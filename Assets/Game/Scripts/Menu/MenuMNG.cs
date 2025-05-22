using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [Header("Location")]
    [SerializeField] private GameObject _locationNameObject;
    [SerializeField] private CanvasGroup _locationCanvasGroup;
    [SerializeField] private TextMeshProUGUI _locationText;
    [SerializeField] private Button _closeButtonLocation;
    [Header("LevelUp")]
    [SerializeField] private GameObject _levelNotificationObject;
    [SerializeField] private CanvasGroup _levelNotificationCanvasGroup;
    [SerializeField] private TextMeshProUGUI _levelNotificationText;
    [SerializeField] private Button _closeButtonLevelUp;
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
    public GameObject Freeze { get => _freeze; set => _freeze = value; }

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
        _locationCanvasGroup.alpha = 0;
        _locationCanvasGroup.interactable = false;
        _locationCanvasGroup.blocksRaycasts = false;
        _levelNotificationCanvasGroup.alpha = 0;
        _levelNotificationCanvasGroup.interactable = false;
        _levelNotificationCanvasGroup.blocksRaycasts = false;
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
        _exitMenu.onClick.AddListener(()=> GameDataMNG.Instance.SaveGame());
        _exitGame.onClick.AddListener(()=> GameMNG.Instance.OnExitGame());
        _exitGame.onClick.AddListener(()=> GameDataMNG.Instance.SaveGame());
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
        _teamButton.onClick.AddListener(() => SetInnactiveDownPanel());
        _teamButton.onClick.AddListener(() => UnitsPanelUI.Instance.OnMenuLoad());
        _teamBackButton.onClick.AddListener(() => GlobalWindowController.Instance.ShowGlobalCanvas());
        _teamBackButton.onClick.AddListener(() => SetActiveDownPanel());
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
        Freeze.SetActive(_boolDirection);
    }

    public void ShowInfo()
    {
        _currentActiveWindow = _InfoPanel;
        Freeze.SetActive(true);
    }

    private bool _isAnimating = false;

    public void CallTask()
    {
        if (_isAnimating) return;

        _isAnimating = true;

        if (_taskVisibility == false)
        {
            _task.transform.DOLocalMoveX(_task.transform.localPosition.x - 500, 0.8f)
                .SetEase(Ease.InOutExpo)
                .OnComplete(() => _isAnimating = false);
        }
        else
        {
            _task.transform.DOLocalMoveX(_task.transform.localPosition.x + 500, 0.8f)
                .SetEase(Ease.InOutExpo)
                .OnComplete(() => _isAnimating = false);
        }

        _taskVisibility = !_taskVisibility;
    }

    public void ChangeVisibilityOfDownPanel()
    {
        _downPanel.SetActive(!_downPanel.activeSelf);
    }

    public void SetActiveDownPanel()
    {
        _downPanel.SetActive(true);
    }
    public void SetInnactiveDownPanel()
    {
        _downPanel.SetActive(false);
    }

    public static LTDescr delay;


    public void SetupLocation(string name)
    {
        _closeButtonLocation.onClick.AddListener(()=> HideLocation(0f));
        _locationText.text = name;
    }

    public void ShowLocation()
    {
        _locationNameObject.SetActive(true);
        _locationCanvasGroup.DOFade(1f, 0.6f).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(2f, () =>
                {
                    HideLocation(0.8f);
                });
            });

        _locationCanvasGroup.interactable = true;
        _locationCanvasGroup.blocksRaycasts = true;
    }


    public void HideLocation(float timer)
    {
        _locationCanvasGroup.DOFade(0f, timer)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                _locationNameObject.SetActive(false);
                _locationCanvasGroup.interactable = false;
                _locationCanvasGroup.blocksRaycasts = false;
            });
    }


    public void SetupLevelNotification(string content)
    {
        _levelNotificationText.text = content;
        GameWindowController.Instance.Button.onClick.AddListener(() => ShowLevelNotification());
        _closeButtonLevelUp.onClick.AddListener(() => HideLevelNotification(0f));
    }

    public void ShowLevelNotification()
    {
        GameWindowController.Instance.Button.onClick.RemoveAllListeners();
        _levelNotificationObject.SetActive(true);
        _levelNotificationCanvasGroup.DOFade(1f, 0.6f).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(8f, () =>
                {
                    HideLevelNotification(2f);
                });
            });

        _levelNotificationCanvasGroup.interactable = true;
        _levelNotificationCanvasGroup.blocksRaycasts = true;
    }

    public void HideLevelNotification(float timer)
    {

        _levelNotificationCanvasGroup.DOFade(0f, timer)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                _levelNotificationObject.SetActive(false);
                _levelNotificationCanvasGroup.interactable = false;
                _levelNotificationCanvasGroup.blocksRaycasts = false;
            });
    }
}
