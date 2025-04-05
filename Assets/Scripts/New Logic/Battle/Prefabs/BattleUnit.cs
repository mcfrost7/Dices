using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _moralText;
    [SerializeField] private TextMeshProUGUI _powerText;
    [SerializeField] private Image _unitImage;
    [SerializeField] private Image _diceImage;
    [SerializeField] private Button _actionTrigger;
    [SerializeField] private RectTransform _linePoint;
    [SerializeField] private RectTransform _arrow;


    private NewUnitStats unitData;

    public NewUnitStats UnitData { get => unitData; set => unitData = value; }
    public RectTransform LinePoint { get => _linePoint; set => _linePoint = value; }
    public RectTransform Arrow { get => _arrow; set => _arrow = value; }
    public Image DiceImage { get => _diceImage; set => _diceImage = value; }
    public Button ActionTrigger { get => _actionTrigger; set => _actionTrigger = value; }
    public TextMeshProUGUI PowerText { get => _powerText; set => _powerText = value; }

    public void SetupUnit(NewUnitStats newUnitStats)
    {
        _healthText.text = newUnitStats._current_health.ToString() + "/" + newUnitStats._health.ToString();
        _moralText.text = newUnitStats._moral.ToString();
        _powerText.text = CalculateSidePowerWithBuffs(newUnitStats, newUnitStats._dice.GetCurrentSide()).ToString();
        _unitImage.sprite = newUnitStats._dice._diceConfig._unitSprite;
        DiceImage.sprite = newUnitStats._dice.GetCurrentSide().sprite;
        ActionTrigger.enabled = false;
        UnitData = newUnitStats;
    }
    public void SetupEnemy(NewUnitStats newUnitStats)
    {
        _healthText.text = newUnitStats._current_health.ToString() + "/" + newUnitStats._health.ToString();
        _unitImage.sprite = newUnitStats._dice._diceConfig._unitSprite;
        int randSide = Random.Range(0, 6);
        DiceSide diceSide = newUnitStats._dice._diceConfig.sides[randSide];
        newUnitStats._dice.SetCurrentSide(diceSide);
        DiceImage.sprite = newUnitStats._dice.GetCurrentSide().sprite;
        _powerText.text = newUnitStats._dice.GetCurrentSide().power.ToString();   
        ActionTrigger.enabled = false;
        UnitData = newUnitStats;
    }

    public int CalculateSidePowerWithBuffs(NewUnitStats clickedUnit, DiceSide diceSide)
    {
        // Базовая сила стороны
        int basePower = diceSide.power;

        // Если базовая сила -1, это означает специальную сторону без числового значения
        if (basePower == -1)
        {
            return -1;
        }

        // Получаем тип стороны куба
        ActionType sideType = diceSide.actionType;

        // Суммарный бонус от баффов
        int buffBonus = 0;

        foreach (BuffConfig buff in clickedUnit._buffs)
        {
            // Проверяем, содержит ли бафф соответствующий тип действия
            if (buff.buffType.Contains(sideType))
            {
                buffBonus += buff.buffPower;
            }
        }

        // Возвращаем итоговое значение (базовая сила + бонус от баффов)
        return basePower + buffBonus;
    }
}
