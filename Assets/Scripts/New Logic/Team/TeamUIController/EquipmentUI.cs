using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    public static EquipmentUI Instance { get; private set; }

    [SerializeField] private List<GameObject> _diceSides;

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

    private void Start()
    {
        UnitsPanelUI.OnUnitSelected += SetupInfo;
    }

    public void SetupInfo(NewUnitStats _clickedUnit)
    {
        if (_diceSides.Count > 0)
        {
            for (int i = 0; i < _diceSides.Count; i++)
            {
                _diceSides[i].GetComponentInChildren<TextMeshProUGUI>().enabled = false;
                _diceSides[i].GetComponent<Image>().sprite = _clickedUnit._dice._diceConfig.sides[i].sprite;
                int finalPower = CalculateSidePower(_clickedUnit, i) + _clickedUnit._dice._diceConfig.sides[i].power;

                if (finalPower != -1)
                {
                    _diceSides[i].GetComponentInChildren<TextMeshProUGUI>().enabled = true;
                    _diceSides[i].GetComponentInChildren<TextMeshProUGUI>().text = finalPower.ToString();
                }
            }
        }
    }

    public int CalculateSidePower(NewUnitStats clickedUnit, int sideIndex)
    {
        if (clickedUnit._buffs == null)
            return 0;
        if (sideIndex < 0 || sideIndex >= clickedUnit._dice._diceConfig.sides.Count)
        {
            Debug.LogError("Side index out of range");
            return 0;
        }
        ActionType sideType = clickedUnit._dice._diceConfig.sides[sideIndex].actionType;
        int buffBonus = 0;
        int itemBonus = 0;

        foreach (BuffConfig buff in clickedUnit._buffs)
        {
            if (buff.buffType == sideType)
            {
                buffBonus += buff.buffPower;
            }
        }
        foreach (var item in clickedUnit._dice._items)
        {
            if (item.actionType == sideType && item != null)
            {
                switch (item.sideAffect)
                {
                    case ItemSideAffect.Nearest:
                        if (sideIndex+1 == item.inventoryPosition)
                        {
                            itemBonus += item.power;
                        }
                        break;

                    case ItemSideAffect.Even:
                        if ((sideIndex + 1) % 2 == 0)
                        {
                            itemBonus += item.power;
                        }
                        break;

                    case ItemSideAffect.Odd:
                        if ((sideIndex + 1) % 2 != 0)
                        {
                            itemBonus += item.power;
                        }
                        break;

                    case ItemSideAffect.All:
                        itemBonus += item.power;
                        break;

                    case ItemSideAffect.Touching:
                        switch (item.inventoryPosition)
                        {
                            case -1:
                                break;
                            case 1:
                                if (sideIndex  == 0 || sideIndex == 3)
                                {
                                    itemBonus += item.power;
                                }
                                break;
                            case 2:
                                if (sideIndex == 1 || sideIndex == 0)
                                {
                                    itemBonus += item.power;
                                }
                                break;
                            case 3:
                                if (sideIndex  == 2 || sideIndex == 1)
                                {
                                    itemBonus += item.power;
                                }
                                break;
                            case 4:
                                if (sideIndex  == 3 || sideIndex == 2)
                                {
                                    itemBonus += item.power;
                                }
                                break;
                        }
                        break;

                    case ItemSideAffect.None:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        clickedUnit._dice.GetCurrentSide().bonus = itemBonus + buffBonus;
        return clickedUnit._dice.GetCurrentSide().bonus;
    }


}
