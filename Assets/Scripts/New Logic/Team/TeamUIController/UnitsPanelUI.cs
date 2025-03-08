using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitsPanelUI : MonoBehaviour
{
    public static event Action<NewUnitStats> OnUnitSelected;

    [SerializeField] private GameObject _unitOnPanelPrefab;
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _addButton;

    private NewUnitStats _currentUnit; 
    public NewUnitStats CurrentUnit { get => _currentUnit; set => _currentUnit = value; }

    public void OnMenuLoad()
    {
        DrawUnitsOnPanel();
    }

    private void DrawUnitsOnPanel()
    {
        ClearPanel();
        if (_unitOnPanelPrefab != null && _panel != null)
        {
            foreach (NewUnitStats unit in GameDataMNG.Instance.PlayerData.PlayerUnits)
            {
                GameObject _unitOnPanel = Instantiate(_unitOnPanelPrefab, _panel.transform);
                Image image = _unitOnPanel.GetComponent<Image>();
                if (image != null) 
                {
                    image.sprite = unit._dice.diceConfig._unitSprite;
                }
                Button button = _unitOnPanel.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => OnUnitSelected?.Invoke(unit));
                }
            }
        }
    }

    private void ClearPanel()
    {
        if (_panel != null) 
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

}
