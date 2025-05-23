using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitsPanelUI : MonoBehaviour
{
    public static UnitsPanelUI Instance { get; private set; }

    public static event Action<NewUnitStats> OnUnitSelected;
    public static event Action SceneLoaded;

    [SerializeField] private UIUnit _unitOnPanelPrefab;
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _addButton;
    [SerializeField] private TextMeshProUGUI _countUnits;

    private NewUnitStats _currentUnit; 
    public NewUnitStats CurrentUnit { get => _currentUnit; set => _currentUnit = value; }

    private List<NewUnitStats> playerUnits;
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

    public void OnMenuLoad()
    {

        playerUnits = TeamMNG.Instance.GetPlayerUnits();
        DrawUnitsOnPanel();
        SetUpSceneWithLastUnit();
        CampSetup();
        SceneLoaded?.Invoke();
    }
    private void CampSetup()
    {
        if (GameDataMNG.Instance.CurrentTile != null)
        {
            if (GameDataMNG.Instance.CurrentTile.campSettings.isReinforceAvailable == true)
                _addButton.SetActive(playerUnits.Count < 5);
            else _addButton.SetActive(false);
        }
        else _addButton.SetActive(false);

    }

    private void SetUpSceneWithLastUnit()
    {
        if (playerUnits.Count > 0)
        {
            NewUnitStats lastUnit = playerUnits[playerUnits.Count - 1];
            CurrentUnit = lastUnit; // Обновляем текущий юнит
            OnUnitSelected?.Invoke(lastUnit); // Вызываем событие выбора
        }
    }

    private void DrawUnitsOnPanel()
    {
        ClearPanel();
        foreach (NewUnitStats unit in GameDataMNG.Instance.PlayerData.PlayerUnits)
        {
            UIUnit _unitOnPanel = Instantiate(_unitOnPanelPrefab, _panel.transform);
            _unitOnPanel.Initialize(unit, () => OnUnitClick(unit));
        }
        _countUnits.text = GameDataMNG.Instance.PlayerData.PlayerUnits.Count.ToString() + "/5";

    }
    public void OnUnitClick(NewUnitStats unit)
    {
        SFXManager.Instance.PlayUISound(UISoundsEnum.UnitSelected);
        CurrentUnit = unit;
        OnUnitSelected?.Invoke(unit);
    }

    private void ClearPanel()
    {
        foreach (Transform child in _panel.transform)
        {
            if (child.gameObject.name != "AddButton")
            {
                Destroy(child.gameObject);
            }
        }   
    }


    public void AddButtonVisibility()
    {
        _addButton.SetActive(true);
    }
}
