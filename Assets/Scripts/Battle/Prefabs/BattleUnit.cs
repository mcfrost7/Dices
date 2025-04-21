using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _moralText;
    [SerializeField] private TextMeshProUGUI _defenseText;
    [SerializeField] private TextMeshProUGUI _powerText;
    [SerializeField] private Image _unitImage;
    [SerializeField] private Image _diceImage;
    [SerializeField] private Button _actionTrigger;
    [SerializeField] private RectTransform _linePoint;
    [SerializeField] private RectTransform _arrow;

    // Add visual indicator for selection
    [SerializeField] private GameObject _selectionIndicator;

    private NewUnitStats unitData;
    private bool isSelected = false;
    private bool isEnemy = false;
    private bool isUsed = false;
    public NewUnitStats UnitData { get => unitData; set => unitData = value; }
    public RectTransform LinePoint { get => _linePoint; set => _linePoint = value; }
    public RectTransform Arrow { get => _arrow; set => _arrow = value; }
    public Image DiceImage { get => _diceImage; set => _diceImage = value; }
    public Button ActionTrigger { get => _actionTrigger; set => _actionTrigger = value; }
    public TextMeshProUGUI PowerText { get => _powerText; set => _powerText = value; }
    public bool IsEnemy { get => isEnemy; set => isEnemy = value; }
    public bool IsUsed { get => isUsed; set => isUsed = value; }
    public bool IsSelected { get => isSelected; set => isSelected = value; }

    private void Awake()
    {
        _actionTrigger.onClick.RemoveAllListeners();
        _actionTrigger.onClick.AddListener(OnActionTriggerClicked);
    }

    public void AllowSelfSelection()
    {
        if (!IsUsed && !IsEnemy)
        {
            DiceSide currentSide = UnitData._dice.GetCurrentSide();
            if (currentSide.ActionSide == ActionSide.Ally)
            {
                BattleActionManager.Instance.SetSelectedUnit(this);
                BattleActionManager.Instance.SetTargetUnit(this);
            }
        }
    }
    private void OnActionTriggerClicked()
    {
        if (isEnemy)
        {
            ToggleEnemySelection();
        }
        else if (BattleActionManager.Instance.IsWaitingForTarget)
        {
            BattleActionManager.Instance.SetTargetUnit(this);
        }
        else
        {
            DiceSide currentSide = UnitData._dice.GetCurrentSide();
            if (currentSide.ActionSide == ActionSide.Ally)
            {
                ToggleSelection();
            }
            else
            {
                ToggleSelection();
            }
        }
    }

    public void SetupUnit(NewUnitStats newUnitStats)
    {
        IsSelected = false;
        IsUsed = false;
        IsEnemy = false;
        SetupCommon(newUnitStats);
        _moralText.text = newUnitStats._moral.ToString();
        _powerText.text = CalculateSidePower(newUnitStats, newUnitStats._dice.GetCurrentSide()).ToString();
    }

    public void SetupEnemy(NewUnitStats newUnitStats)
    {
        IsEnemy = true;
        int randSide = Random.Range(0, 6);
        DiceSide diceSide = newUnitStats._dice._diceConfig.sides[randSide];
        newUnitStats._dice.SetCurrentSide(diceSide);
        SetupCommon(newUnitStats);
        _powerText.text = newUnitStats._dice.GetCurrentSide().power.ToString();
    }

    private void SetupCommon(NewUnitStats newUnitStats)
    {
        _healthText.text = $"{newUnitStats._current_health}/{newUnitStats._health}";
        _unitImage.sprite = newUnitStats._dice._diceConfig._unitSprite;
        DiceImage.sprite = newUnitStats._dice.GetCurrentSide().sprite;
        UnitData = newUnitStats;
        UnitData._current_defense = 0;
        SetSelectionState(IsSelected);
    }

    public int CalculateSidePower(NewUnitStats _clickedUnit, DiceSide _diceSide)
    {
        return EquipmentUI.Instance.CalculateSidePower(_clickedUnit,_diceSide.sideIndex) + _diceSide.power;
    }

    public void ToggleSelection()
    {
        if (IsUsed && !BattleDiceManager.Instance.AllowMultipleSelections)
        {
            return;
        }
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

            IsSelected = !IsSelected;
        }
        else
        {
            IsSelected = !IsSelected;
        }

        _selectionIndicator?.SetActive(IsSelected);
        BattleDiceManager.Instance.HandleUnitSelectionChanged(this);

        if (allowMultiple && IsSelected)
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
        if (IsSelected != selected)
        {
            IsSelected = selected;
            if (_selectionIndicator != null)
            {
                _selectionIndicator.SetActive(selected);
            }
        }
    }


    public void RefreshUnitUI()
    {
        if (UnitData == null) return;

        _healthText.text = $"{UnitData._current_health}/{UnitData._health}";

        if (!IsEnemy)
        {
            _moralText.text = UnitData._moral.ToString();
            _powerText.text = CalculateSidePower(UnitData, UnitData._dice.GetCurrentSide()).ToString();
        }
        else
        {
            _powerText.text = UnitData._dice.GetCurrentSide().power.ToString();
        }

        DiceImage.sprite = UnitData._dice.GetCurrentSide().sprite;
        UpdateHealthVisuals();
        UpdateDefenseUI();
    }
    public void UpdateDefenseUI()
    {
        if (_defenseText == null) return;

        if (UnitData._current_defense > 0)
        {
            _defenseText.gameObject.SetActive(true);
            _defenseText.text = UnitData._current_defense.ToString();

            // Визуальные эффекты для щита
            _defenseText.color = UnitData._current_defense >= UnitData._current_defense ?
                Color.cyan : // Максимальный щит
                new Color(0.5f, 0.8f, 1f); // Обычный щит
        }
        else
        {
            _defenseText.gameObject.SetActive(false);
        }
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
        IsUsed = true; ;
        SetSelectionState(false);
        if (_unitImage != null)
        {
            _unitImage.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
        }
        if (DiceImage != null)
        {
            DiceImage.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
        }
    }

    public void EnableUnitForNewTurn()
    {
        ActionTrigger.enabled = true;
        isUsed = false;
        if  ( _selectionIndicator != null )
            _selectionIndicator.SetActive(false);
        if (_unitImage != null)
        {
            _unitImage.color = Color.white;
        }

        if (DiceImage != null)
        {
            DiceImage.color = Color.white;
        }

        RefreshUnitUI();
    }

    public void RefreshDiceUI()
    {
        if (UnitData == null) return;

        DiceImage.sprite = UnitData._dice.GetCurrentSide().sprite;

        if (!IsEnemy)
        {
            _powerText.text = CalculateSidePower(UnitData, UnitData._dice.GetCurrentSide()).ToString();
        }
        else
        {
            _powerText.text = UnitData._dice.GetCurrentSide().power.ToString();
        }
    }

}