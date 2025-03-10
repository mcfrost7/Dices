using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitsPanelUI : MonoBehaviour
{
    public static UnitsPanelUI Instance { get; private set; }

    public static event Action<NewUnitStats> OnUnitSelected;

    [SerializeField] private UIUnit _unitOnPanelPrefab;
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _addButton;

    private NewUnitStats _currentUnit; 
    public NewUnitStats CurrentUnit { get => _currentUnit; set => _currentUnit = value; }
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
        DrawUnitsOnPanel();
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
