using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private UIUnit _prefab;
    [SerializeField] private Transform _panel;

    private void Start()
    {
        UnitsPanelUI.OnUnitSelected += SetUpInfo;
    }

    private void SetUpInfo(NewUnitStats _unit)
    {
        ClearPanel();

        for (int i = 0; i < _unit._upgrade_list.Count;i++)
        {
            UIUnit _unitOnPanel = Instantiate(_prefab, _panel);
            _unitOnPanel.InitializeUpgrade(TeamMNG.Instance.GetUnitByID(_unit._upgrade_list[i]),_unit);
        }
    }
    private void ClearPanel()
    {
        foreach (Transform child in _panel.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
