using UnityEngine;
using System.Collections.Generic;
using System;



public class GlobalWindowController : MonoBehaviour
{
    public static GlobalWindowController Instance { get; private set; }
    public GameObject GlobalCanvas { get => _globalCanvas; set => _globalCanvas = value; }

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
        ShowMenu();
    }

    public void ShowMenu() => SwitchCanvas(_menu);
    public void ShowGlobalCanvas() => SwitchCanvas(GlobalCanvas);
    public void ShowTeam() => SwitchCanvas(_team);
    public void ShowBattle() => SwitchCanvas(_battle);
    public void ShowRoulette() => SwitchCanvas(_roulette);

    // Метод для показа настроек с сохранением предыдущего экрана
    public void ShowSettings()
    {
        if (_currentActiveCanvas != _settings)
        {
            _canvasHistory.Push(_currentActiveCanvas); // Сохраняем предыдущий экран
        }
        SwitchCanvas(_settings);
    }

    // Метод для возврата на предыдущий экран
    public void GoBack()
    {
        if (_canvasHistory.Count > 0)
        {
            GameObject previousCanvas = _canvasHistory.Pop();
            SwitchCanvas(previousCanvas, false); // Не добавляем в историю
        }
    }

    // Универсальный метод для переключения между канвасами
    private void SwitchCanvas(GameObject targetCanvas, bool addToHistory = true)
    {
        if (_currentActiveCanvas == targetCanvas)
            return;

        if (addToHistory && _currentActiveCanvas != null)
        {
            _canvasHistory.Push(_currentActiveCanvas);
        }

        HideAllCanvases();
        targetCanvas.SetActive(true);
        _currentActiveCanvas = targetCanvas;
        OnCanvasSwitched?.Invoke(_currentActiveCanvas);
    }


    // Метод для скрытия всех канвасов
    private void HideAllCanvases()
    {
        _menu.SetActive(false);
        GlobalCanvas.SetActive(false);
        _settings.SetActive(false);
        _team.SetActive(false);
        _battle.SetActive(false);
        _roulette.SetActive(false);
    }

    // Методы для проверки активности конкретного канваса
    public bool IsMenuActive() => _menu.activeSelf;
    public bool IsGlobalCanvasActive() => GlobalCanvas.activeSelf;
    public bool IsSettingsActive() => _settings.activeSelf;
    public bool IsTeamActive() => _team.activeSelf;
    public bool IsBattleActive() => _battle.activeSelf;
    public bool IsRouletteActive() => _roulette.activeSelf;

}
