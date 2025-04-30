using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
                    _diceSides[i].GetComponentInChildren<TooltipTrigger>().Header = _diceSides[i].GetComponentInChildren<TooltipTrigger>().GetActionTypeName(_clickedUnit._dice._diceConfig.sides[i].actionType);
                    _diceSides[i].GetComponentInChildren<TooltipTrigger>().Content = SetupDiceSideTooltip(_clickedUnit._dice._diceConfig.sides[i]);
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
            if (item.ActionType == sideType && item != null)
            {
                switch (item.SideAffect)
                {
                    case ItemSideAffect.Nearest:
                        if (sideIndex+1 == item.InventoryPosition)
                        {
                            itemBonus += item.Power;
                        }
                        break;

                    case ItemSideAffect.Even:
                        if ((sideIndex + 1) % 2 == 0)
                        {
                            itemBonus += item.Power;
                        }
                        break;

                    case ItemSideAffect.Odd:
                        if ((sideIndex + 1) % 2 != 0)
                        {
                            itemBonus += item.Power;
                        }
                        break;

                    case ItemSideAffect.All:
                        itemBonus += item.Power;
                        break;

                    case ItemSideAffect.Touching:
                        switch (item.InventoryPosition)
                        {
                            case -1:
                                break;
                            case 1:
                                if (sideIndex  == 0 || sideIndex == 3)
                                {
                                    itemBonus += item.Power;
                                }
                                break;
                            case 2:
                                if (sideIndex == 1 || sideIndex == 0)
                                {
                                    itemBonus += item.Power;
                                }
                                break;
                            case 3:
                                if (sideIndex  == 2 || sideIndex == 1)
                                {
                                    itemBonus += item.Power;
                                }
                                break;
                            case 4:
                                if (sideIndex  == 3 || sideIndex == 2)
                                {
                                    itemBonus += item.Power;
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
        clickedUnit._dice._diceConfig.sides[sideIndex].bonus = itemBonus + buffBonus;
        return clickedUnit._dice._diceConfig.sides[sideIndex].bonus;
    }

    public string SetupDiceSideTooltip(DiceSide diceSide)
    {
        StringBuilder content = new StringBuilder();

        if (diceSide == null || diceSide.actionType == ActionType.None)
        {
            return "Пустая сторона";
        }

        int totalPower = diceSide.power + diceSide.bonus;

        switch (diceSide.actionType)
        {
            case ActionType.Attack:
                content.Append($"Наносит {totalPower} урона");
                if (diceSide.bonus != 0)
                    content.Append($" (база {diceSide.power} + бонус {diceSide.bonus})");
                break;

            case ActionType.Defense:
                content.Append($"Даёт {totalPower} защиты");
                if (diceSide.bonus != 0)
                    content.Append($" (база {diceSide.power} + бонус {diceSide.bonus})");
                break;

            case ActionType.Heal:
                content.Append($"Восстанавливает {totalPower} здоровья");
                if (diceSide.bonus != 0)
                    content.Append($" (база {diceSide.power} + бонус {diceSide.bonus})");
                break;

            case ActionType.LifeSteal:
                content.Append($"Крадёт {totalPower} здоровья у врага");
                if (diceSide.bonus != 0)
                    content.Append($" (база {diceSide.power} + бонус {diceSide.bonus})");
                break;

            case ActionType.Moral:
                content.Append($"Повышает мораль на {totalPower}");
                if (diceSide.bonus != 0)
                    content.Append($" (база {diceSide.power} + бонус {diceSide.bonus})");
                break;

            case ActionType.ShieldBash:
                content.Append($"Наносит ({totalPower} + накопленный щит) урона, накопленный щит пропадает");
                if (diceSide.bonus != 0)
                    content.Append($" (база {diceSide.power} + бонус {diceSide.bonus})");
                break;

            case ActionType.HealthAttack:
                content.Append($"Наносит {totalPower} урона, ваш юнит теряет {totalPower} здоровья");
                if (diceSide.bonus != 0)
                    content.Append($" (база {diceSide.power} + бонус {diceSide.bonus})");
                break;
        }
        if (diceSide.ActionSide != ActionSide.None)
        {
            content.Append("\nЦель: ");
            content.Append(diceSide.ActionSide == ActionSide.Ally ? "союзник" : "враг");
        }

        // Добавляем информацию о длительности, если она есть
        if (diceSide.duration > 1000)
        {
            content.Append($"\nДлительность: {diceSide.duration} ход");
            if (diceSide.duration > 1) content.Append("а");
            if (diceSide.duration > 4) content.Append("ов");
        }

        return content.ToString();
    }
}
