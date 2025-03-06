using UnityEngine;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _globalCanvas;
    [SerializeField] private GameObject _settings;
    [SerializeField] private GameObject _team;
    [SerializeField] private GameObject _battle;

    private GameObject _currentActiveCanvas;
    private Stack<GameObject> _canvasHistory = new Stack<GameObject>();

    private void Awake()
    {
        // По умолчанию активируем меню и деактивируем остальные канвасы
        ShowMenu();
    }

    public void ShowMenu() => SwitchCanvas(_menu);
    public void ShowGlobalCanvas() => SwitchCanvas(_globalCanvas);
    public void ShowTeam() => SwitchCanvas(_team);
    public void ShowBattle() => SwitchCanvas(_battle);

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
    }

    // Метод для скрытия всех канвасов
    private void HideAllCanvases()
    {
        _menu.SetActive(false);
      //  _globalCanvas.SetActive(false);
        _settings.SetActive(false);
        _team.SetActive(false);
        _battle.SetActive(false);
    }

    // Методы для проверки активности конкретного канваса
    public bool IsMenuActive() => _menu.activeSelf;
    public bool IsGlobalCanvasActive() => _globalCanvas.activeSelf;
    public bool IsSettingsActive() => _settings.activeSelf;
    public bool IsTeamActive() => _team.activeSelf;
    public bool IsBattleActive() => _battle.activeSelf;
}
