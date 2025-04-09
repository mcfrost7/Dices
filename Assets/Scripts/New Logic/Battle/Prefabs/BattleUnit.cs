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

    // Add visual indicator for selection
    [SerializeField] private Image _selectionIndicator;

    private NewUnitStats unitData;
    private bool isSelected = false;
    private bool isEnemy = false;
    public NewUnitStats UnitData { get => unitData; set => unitData = value; }
    public RectTransform LinePoint { get => _linePoint; set => _linePoint = value; }
    public RectTransform Arrow { get => _arrow; set => _arrow = value; }
    public Image DiceImage { get => _diceImage; set => _diceImage = value; }
    public Button ActionTrigger { get => _actionTrigger; set => _actionTrigger = value; }
    public TextMeshProUGUI PowerText { get => _powerText; set => _powerText = value; }
    public bool IsSelected { get => isSelected; }
    public bool IsEnemy { get => isEnemy; set => isEnemy = value; }

    private void Awake()
    {
        _actionTrigger.onClick.RemoveAllListeners();
        _actionTrigger.onClick.AddListener(OnActionTriggerClicked);

        if (_selectionIndicator != null)
        {
            _selectionIndicator.gameObject.SetActive(false);
        }
    }

    private void OnActionTriggerClicked()
    {
        if (isEnemy)
        {
            ToggleEnemySelection();
        }
        else
        {
            ToggleSelection();
        }
    }

    public void SetupUnit(NewUnitStats newUnitStats)
    {
        IsEnemy = false;
        SetupCommon(newUnitStats);
        _moralText.text = newUnitStats._moral.ToString();
        _powerText.text = CalculateSidePowerWithBuffs(newUnitStats, newUnitStats._dice.GetCurrentSide()).ToString();
        ActionTrigger.enabled = true; // Enable button for selection
    }

    public void SetupEnemy(NewUnitStats newUnitStats)
    {
        IsEnemy = true;
        int randSide = Random.Range(0, 6);
        DiceSide diceSide = newUnitStats._dice._diceConfig.sides[randSide];
        newUnitStats._dice.SetCurrentSide(diceSide);
        SetupCommon(newUnitStats);
        _powerText.text = newUnitStats._dice.GetCurrentSide().power.ToString();
        ActionTrigger.enabled = false;
    }

    private void SetupCommon(NewUnitStats newUnitStats)
    {
        _healthText.text = $"{newUnitStats._current_health}/{newUnitStats._health}";
        _unitImage.sprite = newUnitStats._dice._diceConfig._unitSprite;
        DiceImage.sprite = newUnitStats._dice.GetCurrentSide().sprite;
        UnitData = newUnitStats;
        UpdateHealthVisuals();
    }

    public int CalculateSidePowerWithBuffs(NewUnitStats clickedUnit, DiceSide diceSide)
    {
        int basePower = diceSide.power;
        if (basePower == -1)
        {
            return -1;
        }
        ActionType sideType = diceSide.actionType;
        int buffBonus = 0;
        foreach (BuffConfig buff in clickedUnit._buffs)
        {
            if (buff.buffType == sideType)
            {
                buffBonus += buff.buffPower;
            }
        }
        return basePower + buffBonus;
    }

    public void ToggleSelection()
    {
        bool allowMultiple = BattleDiceManager.Instance.AllowMultipleSelections;

        if (!allowMultiple)
        {
            foreach (var unit in BattleController.Instance.UnitsObj)
            {
                if (unit != this && unit.IsSelected)
                {
                    unit.SetSelectionState(false);
                }
            }

            isSelected = !isSelected;
        }
        else
        {
            isSelected = !isSelected;
        }

        _selectionIndicator?.gameObject.SetActive(isSelected);
        BattleDiceManager.Instance.HandleUnitSelectionChanged(this);

        if (allowMultiple && isSelected)
        {
            BattleDiceManager.Instance.NotifyUnitSelected(this);
        }
    }

    public void ToggleEnemySelection()
    {
        BattleActionManager.Instance.SetTargetUnit(this);
    }

    public void SetSelectionState(bool selected)
    {
        if (isSelected != selected)
        {
            isSelected = selected;
            if (_selectionIndicator != null)
            {
                _selectionIndicator.gameObject.SetActive(isSelected);
            }
        }
    }

    // Добавьте эти методы в класс BattleUnit

    public void RefreshUnitUI()
    {
        if (UnitData == null) return;

        // Обновляем здоровье
        _healthText.text = $"{UnitData._current_health}/{UnitData._health}";

        // Для своих юнитов обновляем мораль и силу с учетом баффов
        if (!IsEnemy)
        {
            _moralText.text = UnitData._moral.ToString();
            _powerText.text = CalculateSidePowerWithBuffs(UnitData, UnitData._dice.GetCurrentSide()).ToString();
        }
        else
        {
            // Для врагов просто обновляем силу текущей стороны кубика
            _powerText.text = UnitData._dice.GetCurrentSide().power.ToString();
        }

        // Обновляем изображение кубика
        DiceImage.sprite = UnitData._dice.GetCurrentSide().sprite;

        // Визуальная обратная связь при низком здоровье
        UpdateHealthVisuals();
    }


    private void UpdateHealthVisuals()
    {
        float healthPercentage = (float)UnitData._current_health / UnitData._health;

        // Меняем цвет текста здоровья в зависимости от % здоровья
        if (healthPercentage < 0.4f)
        {
            _healthText.color = Color.red;
        }
        else if (healthPercentage < 0.7f)
        {
            _healthText.color = Color.yellow;
        }
        else
        {
            _healthText.color = Color.green;
        }
    }
    public void DisableUnitAfterAction()
    {
        // Выключаем интерактивность
        ActionTrigger.enabled = false;

        // Убираем выделение
        SetSelectionState(false);

        // Затемняем изображение
        if (_unitImage != null)
        {
            _unitImage.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
        }

        // Затемняем кубик
        if (DiceImage != null)
        {
            DiceImage.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
        }
    }

    public void EnableUnitForNewTurn()
    {
        // Включаем интерактивность
        ActionTrigger.enabled = true;

        // Возвращаем нормальные цвета
        if (_unitImage != null)
        {
            _unitImage.color = Color.white;
        }

        if (DiceImage != null)
        {
            DiceImage.color = Color.white;
        }

        // Обновляем UI
        RefreshUnitUI();
    }

    public void RefreshDiceUI()
    {
        if (UnitData == null) return;

        DiceImage.sprite = UnitData._dice.GetCurrentSide().sprite;

        if (!IsEnemy)
        {
            _powerText.text = CalculateSidePowerWithBuffs(UnitData, UnitData._dice.GetCurrentSide()).ToString();
        }
        else
        {
            _powerText.text = UnitData._dice.GetCurrentSide().power.ToString();
        }
    }

}