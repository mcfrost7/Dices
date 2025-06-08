using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuMNG : MonoBehaviour
{
    [Header("Menu UI")]
    [SerializeField] private GameObject _menuWindow;
    [SerializeField] private Button _menu, _continue, _settings, _exitMenu, _exitGame;

    [Header("Main Menu")]
    [SerializeField] private Button _continueMain, _playMain, _settingsMain, _exitGameMain,_backSettings;

    [Header("Help Windows")]
    [SerializeField] private GameObject _InfoPanel, _freeze;

    [Header("Location")]
    [SerializeField] private GameObject _locationNameObject;
    [SerializeField] private CanvasGroup _locationCanvasGroup;
    [SerializeField] private TextMeshProUGUI _locationText;
    [SerializeField] private Button _closeButtonLocation;

    [Header("Level Up")]
    [SerializeField] private GameObject _levelNotificationObject;
    [SerializeField] private CanvasGroup _levelNotificationCanvasGroup;
    [SerializeField] private TextMeshProUGUI _levelNotificationText;
    [SerializeField] private Button _closeButtonLevelUp;

    [Header("Task Panel")]
    [SerializeField] private GameObject _task;
    [SerializeField] private Button _taskButton;

    [Header("Down Panel")]
    [SerializeField] private GameObject _downPanel;
    [SerializeField] private Button _teamButton, _teamBackButton;

    public static MenuMNG Instance { get; private set; }
    public Button ContinueMain { get => _continueMain; set => _continueMain = value; }
    public GameObject Freeze { get => _freeze; set => _freeze = value; }

    private GameObject _currentActiveWindow;
    private bool _taskVisibility = false;
    private bool _isAnimating = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        SetupAllUI();

        SetCanvasGroupState(_locationCanvasGroup, false, 0);
        SetCanvasGroupState(_levelNotificationCanvasGroup, false, 0);
    }

    public void AddButtonListener(Button button, Action action, bool playSound = true)
    {
        button.onClick.AddListener(() =>
        {
            if (playSound)
                SFXManager.Instance.PlayUISound(UISoundsEnum.Click);
            action.Invoke();
        });
    }

    private void SetupAllUI()
    {
        SetupMenuButtons();
        SetupMainButtons();
        SetupTaskPanel();
        SetupDownPanel();
    }

    private void SetupMenuButtons()
    {
        AddButtonListener(_menu, () => ShowMenu(_menuWindow));
        AddButtonListener(_continue, HideWindow);
        AddButtonListener(_settings, SetupSettings);
        AddButtonListener(_exitMenu, () => { 
            SetupMainMenu();
            GameDataMNG.Instance.SaveGame();
            if (CampPanel.Instance.CampVisibility == true)
                CampPanel.Instance.SetCampVisibility(false);
        });
        AddButtonListener(_exitGame, () => { GameMNG.Instance.OnExitGame();
            GameDataMNG.Instance.SaveGame();
            if (CampPanel.Instance.CampVisibility == true)
                CampPanel.Instance.SetCampVisibility(false);
        });
    }

    private void SetupMainButtons()
    {
        AddButtonListener(_continueMain, GameMNG.Instance.OnContinueGame);
        AddButtonListener(_playMain, GameMNG.Instance.OnNewGameStart);
        AddButtonListener(_settingsMain, GlobalWindowController.Instance.ShowSettings);
        AddButtonListener(_exitGameMain, GameMNG.Instance.OnExitGame);
        AddButtonListener(_backSettings, GlobalWindowController.Instance.GoBack);
    }

    private void SetupTaskPanel()
    {
        AddButtonListener(_taskButton, ToggleTask);
    }

    private void SetupDownPanel()
    {
        AddButtonListener(_teamButton, () =>
        {
            GlobalWindowController.Instance.ShowTeam();
            SetDownPanelVisible(false);
            UnitsPanelUI.Instance.OnMenuLoad();
        });

        AddButtonListener(_teamBackButton, () =>
        {
            GlobalWindowController.Instance.ShowGlobalCanvas();
            SetDownPanelVisible(true);
        });
    }

    public void HideWindow()
    {
        CallFreezePanel(false);
        _currentActiveWindow?.SetActive(false);
    }

    private void SetupSettings()
    {
        HideWindow();
        GlobalWindowController.Instance.ShowSettings();
    }

    private void SetupMainMenu()
    {
        _continueMain.interactable = GameMNG.Instance.IsGameLoaded;
        HideWindow();
        GlobalWindowController.Instance.ShowMenu();
    }

    public void ShowMenu(GameObject window)
    {
        _currentActiveWindow = window;
        window.SetActive(true);
        CallFreezePanel(true);
    }

    public void CallFreezePanel(bool isActive)
    {
        _freeze.SetActive(isActive);
    }

    public void ShowInfo()
    {
        _currentActiveWindow = _InfoPanel;
        _freeze.SetActive(true);
    }

    private void ToggleTask()
    {
        if (_isAnimating) return;
        _isAnimating = true;

        float moveX = _task.transform.localPosition.x + (_taskVisibility ? 500 : -500);

        _task.transform.DOLocalMoveX(moveX, 0.8f).SetEase(Ease.InOutExpo)
            .OnComplete(() => _isAnimating = false);

        _taskVisibility = !_taskVisibility;
    }

    public void SetDownPanelVisible(bool visible)
    {
        _downPanel.SetActive(visible);
    }

    public void ChangeVisibilityOfDownPanel() => _downPanel.SetActive(!_downPanel.activeSelf);

    public void SetupLocation(string name)
    {
        _locationText.text = name;
        AddButtonListener(_closeButtonLocation, () => HideLocation(0f), playSound: false);
    }

    public void ShowLocation()
    {
        _locationNameObject.SetActive(true);
        _locationCanvasGroup.DOFade(1f, 0.6f).SetEase(Ease.InOutSine)
            .OnComplete(() => DOVirtual.DelayedCall(2f, () => HideLocation(0.8f)));

        SetCanvasGroupState(_locationCanvasGroup, true);
    }

    public void HideLocation(float duration)
    {
        _locationCanvasGroup.DOFade(0f, duration).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                _locationNameObject.SetActive(false);
                SetCanvasGroupState(_locationCanvasGroup, false);
            });
    }

    public void SetupLevelNotification(string content)
    {
        _levelNotificationText.text = content;
        AddButtonListener(GameWindowController.Instance.Button, ShowLevelNotification);
        AddButtonListener(_closeButtonLevelUp, () => HideLevelNotification(0f));
    }

    public void ShowLevelNotification()
    {
        GameWindowController.Instance.Button.onClick.RemoveAllListeners();
        _levelNotificationObject.SetActive(true);
        _levelNotificationCanvasGroup.DOFade(1f, 0.6f).SetEase(Ease.InOutSine)
            .OnComplete(() => DOVirtual.DelayedCall(8f, () => HideLevelNotification(2f)));

        SetCanvasGroupState(_levelNotificationCanvasGroup, true);
    }

    public void HideLevelNotification(float duration)
    {
        _levelNotificationCanvasGroup.DOFade(0f, duration).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                _levelNotificationObject.SetActive(false);
                SetCanvasGroupState(_levelNotificationCanvasGroup, false);
            });
    }

    private void SetCanvasGroupState(CanvasGroup cg, bool state, float alpha = -1f)
    {
        cg.interactable = state;
        cg.blocksRaycasts = state;
        if (alpha >= 0f) cg.alpha = alpha;
    }
}
