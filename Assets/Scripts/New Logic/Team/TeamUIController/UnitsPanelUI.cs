using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitsPanelUI : MonoBehaviour
{
    public static UnitsPanelUI Instance { get; private set; }

    public static event Action<NewUnitStats> OnUnitSelected;

    [SerializeField] private UIUnit _unitOnPanelPrefab;
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _addButton;

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
        playerUnits = GameDataMNG.Instance.PlayerData.PlayerUnits;
        DrawUnitsOnPanel();
        SetUpSceneWithLastUnit();
         if (playerUnits.Count == 5)
            _addButton.SetActive(false);
        else _addButton.SetActive(true);

    }

    private void SetUpSceneWithLastUnit()
    {
        if (playerUnits.Count > 0)
        {
            NewUnitStats lastUnit = playerUnits[playerUnits.Count - 1];
            CurrentUnit = lastUnit; // ��������� ������� ����
            OnUnitSelected?.Invoke(lastUnit); // �������� ������� ������
        }
    }

    private void DrawUnitsOnPanel()
    {
        ClearPanel();
        foreach (NewUnitStats unit in GameDataMNG.Instance.PlayerData.PlayerUnits)
        {
            UIUnit _unitOnPanel = Instantiate(_unitOnPanelPrefab, _panel.transform);
            _unitOnPanel.Initialize(unit._dice.diceConfig, ()=>OnUnitSelected.Invoke(unit));
        }
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

}
