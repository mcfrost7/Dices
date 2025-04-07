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

    public NewUnitStats UnitData { get => unitData; set => unitData = value; }
    public RectTransform LinePoint { get => _linePoint; set => _linePoint = value; }
    public RectTransform Arrow { get => _arrow; set => _arrow = value; }
    public Image DiceImage { get => _diceImage; set => _diceImage = value; }
    public Button ActionTrigger { get => _actionTrigger; set => _actionTrigger = value; }
    public TextMeshProUGUI PowerText { get => _powerText; set => _powerText = value; }
    public bool IsSelected { get => isSelected; }


    private void Awake()
    {
        // Add click listener to button
        _actionTrigger.onClick.AddListener(ToggleSelection);

        // Initialize selection indicator if it exists
        if (_selectionIndicator != null)
        {
            _selectionIndicator.gameObject.SetActive(false);
        }
    }

    public void SetupUnit(NewUnitStats newUnitStats)
    {
        _healthText.text = newUnitStats._current_health.ToString() + "/" + newUnitStats._health.ToString();
        _moralText.text = newUnitStats._moral.ToString();
        _powerText.text = CalculateSidePowerWithBuffs(newUnitStats, newUnitStats._dice.GetCurrentSide()).ToString();
        _unitImage.sprite = newUnitStats._dice._diceConfig._unitSprite;
        DiceImage.sprite = newUnitStats._dice.GetCurrentSide().sprite;
        ActionTrigger.enabled = true; // Enable button for selection
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

}