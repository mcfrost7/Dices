using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModificatorUI : MonoBehaviour
{
    [SerializeField] private UIUnit _modificatorPrefab;
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _defaultText;


    private void Start()
    {
        UnitsPanelUI.OnUnitSelected += SetUpInfo;
    }

    public void SetUpInfo(NewUnitStats _clickedUnit)
    {
        ClearPanel();
        if (_clickedUnit._buffs.Count > 0)
        {
            for (int i = 0; i < _clickedUnit._buffs.Count; i++)
            {
                UIUnit _modificatorOnPanel = Instantiate(_modificatorPrefab, _panel.transform);
                _modificatorOnPanel.InitializeBuffs(_clickedUnit._buffs[i]);
            }
        }
        else
        {
            _defaultText?.SetActive(true);
        }

    }

    private void ClearPanel()
    {
        foreach (Transform child in _panel.transform)
        {
            Destroy(child.gameObject);
        }
        _defaultText?.SetActive(false);
    }
}
