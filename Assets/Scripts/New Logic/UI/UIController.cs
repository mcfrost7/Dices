using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _globalCanvs;
    [SerializeField] private GameObject _Settings;
    [SerializeField] private GameObject _team;
    [SerializeField] private GameObject _battle;

    private GameObject _currentActiveCanvas;

    private void Awake()
    {
        // По умолчанию активируем меню и деактивируем остальные канвасы
        ShowMenu();
    }

    public void ShowMenu()
    {
        SwitchCanvas(_menu);
    }

    public void ShowGlobalCanvas()
    {
        SwitchCanvas(_globalCanvs);
    }

    public void ShowSettings()
    {
        SwitchCanvas(_Settings);
    }

    public void ShowTeam()
    {
        SwitchCanvas(_team);
    }

    public void ShowBattle()
    {
        SwitchCanvas(_battle);
    }

    // Метод для скрытия всех канвасов
    public void HideAllCanvases()
    {
        _menu.SetActive(false);
        _globalCanvs.SetActive(false);
        _Settings.SetActive(false);
        _team.SetActive(false);
        _battle.SetActive(false);
        _currentActiveCanvas = null;
    }

    // Универсальный метод для переключения между канвасами
    private void SwitchCanvas(GameObject targetCanvas)
    {
        // Если текущий канвас такой же как целевой, ничего не делаем
        if (_currentActiveCanvas == targetCanvas)
            return;

        // Деактивируем все канвасы
        HideAllCanvases();

        // Активируем целевой канвас
        targetCanvas.SetActive(true);
        _currentActiveCanvas = targetCanvas;
    }

    // Методы для проверки активности конкретного канваса
    public bool IsMenuActive() => _menu.activeSelf;
    public bool IsGlobalCanvasActive() => _globalCanvs.activeSelf;
    public bool IsSettingsActive() => _Settings.activeSelf;
    public bool IsTeamActive() => _team.activeSelf;
    public bool IsBattleActive() => _battle.activeSelf;
}