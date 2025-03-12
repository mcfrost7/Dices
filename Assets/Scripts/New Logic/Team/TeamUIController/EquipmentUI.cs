using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{

    [SerializeField] private List<GameObject> _diceSides;
    [SerializeField] private List<GameObject> _itemSides;

    private void Start()
    {
        UnitsPanelUI.OnUnitSelected += SetUpInfo;
    }

    private void SetUpInfo(NewUnitStats _clickedUnit)
    {
        SetUpItems(_clickedUnit);
        if (_diceSides.Count > 0)
        {
            for (int i = 0; i < _diceSides.Count; i++)
            {
                _diceSides[i].GetComponentInChildren<TextMeshProUGUI>().enabled = false;
                _diceSides[i].GetComponent<Image>().sprite = _clickedUnit._dice.diceConfig.sides[i].sprite;

                // �������� �������� � ������ ������
                int finalPower = CalculateSidePowerWithBuffs(_clickedUnit, i);

                if (finalPower != -1)
                {
                    _diceSides[i].GetComponentInChildren<TextMeshProUGUI>().enabled = true;
                    _diceSides[i].GetComponentInChildren<TextMeshProUGUI>().text = finalPower.ToString();
                }
            }
        }
    }

    private void SetUpItems(NewUnitStats _clickedUnit)
    {
        if (_itemSides.Count > 0)
        {
            for (int i = 0; i < _itemSides.Count; i++)
            {
                _itemSides[i].GetComponent<Image>().enabled = false;
                if (_clickedUnit._dice.items.Count > 0)
                {
                    _itemSides[i].SetActive(true);
                    _itemSides[i].GetComponent<Image>().sprite = _clickedUnit._dice.items[i].icon;
                }
            }
        }
    }

    public int CalculateSidePowerWithBuffs(NewUnitStats clickedUnit, int sideIndex)
    {
        // �������� �� ������������ �������
        if (sideIndex < 0 || sideIndex >= clickedUnit._dice.diceConfig.sides.Count)
        {
            Debug.LogError("Side index out of range");
            return 0;
        }

        // ������� ���� �������
        int basePower = clickedUnit._dice.diceConfig.sides[sideIndex].power;

        // ���� ������� ���� -1, ��� �������� ����������� ������� ��� ��������� ��������
        if (basePower == -1)
        {
            return -1;
        }

        // �������� ��� ������� ����
        ActionType sideType = clickedUnit._dice.diceConfig.sides[sideIndex].actionType;

        // ��������� ����� �� ������
        int buffBonus = 0;

        foreach (BuffConfig buff in clickedUnit._buffs)
        {
            // ���������, �������� �� ���� ��������������� ��� ��������
            if (buff.buffType.Contains(sideType))
            {
                buffBonus += buff.buffPower;
            }
        }

        // ���������� �������� �������� (������� ���� + ����� �� ������)
        return basePower + buffBonus;
    }
}
