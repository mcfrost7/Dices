using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedSide : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _content;

    private DiceSide _diceSide;
    public void SetupInfo(BattleUnit _battleUnit)
    {
        _diceSide = _battleUnit.UnitData._dice.GetCurrentSide();
        _image.sprite = _diceSide.sprite;
        _content.text = EquipmentUI.Instance.SetupDiceSideTooltip(_diceSide);
    }

}
