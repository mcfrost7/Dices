using UnityEngine;
using System.Collections.Generic;
using System;



public class GlobalWindowController : MonoBehaviour
{
    public static GlobalWindowController Instance { get; private set; }
    public GameObject GlobalCanvas { get => _globalCanvas; set => _globalCanvas = value; }
    public GameObject CurrentActiveCanvas { get => _currentActiveCanvas; set => _currentActiveCanvas = value; }

    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _globalCanvas;
    [SerializeField] private GameObject _settings;
    [SerializeField] private GameObject _team;
    [SerializeField] private GameObject _battle;
    [SerializeField] private GameObject _roulette;

    private GameObject _currentActiveCanvas;
    private Stack<GameObject> _canvasHistory = new Stack<GameObject>();
    public event Action<GameObject> OnCanvasSwitched;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        ShowMenu();
    }

    public void ShowMenu()
    {
        GlobalSoundManager.Instance.PlayMusic(MusicTheme.Menu);
        SwitchCanvas(_menu);
    }
    public void ShowGlobalCanvas()
    {
        GlobalSoundManager.Instance.PlayMusic(MusicTheme.Main);
        GameDataMNG.Instance.SaveGame();
        SwitchCanvas(GlobalCanvas);
        MenuMNG.Instance.SetDownPanelVisible(true);
        GameDataMNG.Instance.Tutorial.ShowTutorial();
    }
    public void ShowTeam()
    {
        SwitchCanvas(_team);
        GameDataMNG.Instance.Tutorial.ShowTutorial();
    }
    public void ShowBattle()
    {
        GlobalSoundManager.Instance.PlayMusic(MusicTheme.Battle);
        SwitchCanvas(_battle);
        GameDataMNG.Instance.Tutorial.ShowTutorial();
    }
    public void ShowRoulette()
    {
        MenuMNG.Instance.CallFreezePanel(true);
        SwitchCanvas(_roulette);
    }
    public void ShowSettings()
    {
        if (CurrentActiveCanvas != _settings)
        {
            _canvasHistory.Push(CurrentActiveCanvas); 
        }
        SwitchCanvas(_settings);
    }
    public void GoBack()
    {
        if (_canvasHistory.Count > 0)
        {
            GameObject previousCanvas = _canvasHistory.Pop();
            SwitchCanvas(previousCanvas, false); 
        }
    }
    private void SwitchCanvas(GameObject targetCanvas, bool addToHistory = true)
    {
        if (CurrentActiveCanvas == targetCanvas)
            return;

        if (addToHistory && CurrentActiveCanvas != null)
        {
            _canvasHistory.Push(CurrentActiveCanvas);
        }
        CurrentActiveCanvas = targetCanvas;
        HideAllCanvases();
        targetCanvas.SetActive(true);
        OnCanvasSwitched?.Invoke(targetCanvas);
    }


    public void HideAllCanvases()
    {
        _menu.SetActive(false);
        _settings.SetActive(false);
        _team.SetActive(false);
        _battle.SetActive(false);
        _roulette.SetActive(false); 
        if (CurrentActiveCanvas != _roulette)
        {
            GlobalCanvas.SetActive(false);
        }
    }
    public bool IsMenuActive() => _menu.activeSelf;
    public bool IsGlobalCanvasActive() => GlobalCanvas.activeSelf;
    public bool IsSettingsActive() => _settings.activeSelf;
    public bool IsTeamActive() => _team.activeSelf;
    public bool IsBattleActive() => _battle.activeSelf;
    public bool IsRouletteActive() => _roulette.activeSelf;

}
