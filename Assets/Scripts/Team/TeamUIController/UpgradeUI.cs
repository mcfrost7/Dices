using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        bool isCamp = false;
        if (GameDataMNG.Instance.CurrentTile != null)
        {
            var campSettings = GameDataMNG.Instance.CurrentTile.campSettings;
            if (campSettings != null)
            {
                isCamp = campSettings.isUpgradeAvailable;
            }
        }
        ClearPanel();
        foreach (var upgradeID in _unit._upgrade_list)
        {
            var unitOnPanel = Instantiate(_prefab, _panel);
            unitOnPanel.gameObject.SetActive(false);
            var nextUnit = TeamMNG.Instance.GetUnitByID(upgradeID);
            unitOnPanel.InitializeUpgrade(nextUnit, _unit, isCamp);
            unitOnPanel.gameObject.SetActive(true);
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
