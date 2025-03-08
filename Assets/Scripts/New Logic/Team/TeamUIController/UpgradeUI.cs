using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    private void Start()
    {
        UnitsPanelUI.OnUnitSelected += SetUpInfo;
    }

    private void SetUpInfo(NewUnitStats _clickedUnit)
    {

    }
}
