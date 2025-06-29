using DG.Tweening;
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
    [SerializeField] private RectTransform _unitRectTransform;
    [SerializeField] private TooltipTrigger _trigger;



    // Add visual indicator for selection
    [SerializeField] private GameObject _selectionIndicator;

    private NewUnitStats unitData;
    private bool isSelected = false;
    private bool isEnemy = false;
    private bool isUsed = false;
    private Vector3 _originalPosition;
    private Vector3 _selectedOffset = new Vector3(30f, 0, 0);
    private Tween _moveTween;
    private LayoutElement _layoutElement;
    private HorizontalOrVerticalLayoutGroup _parentLayoutGroup;
    private ContentSizeFitter _parentFitter;
    private bool _layoutWasEnabled;

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
        _layoutElement = GetComponent<LayoutElement>();
        _parentLayoutGroup = GetComponentInParent<HorizontalOrVerticalLayoutGroup>();
        _parentFitter = GetComponentInParent<ContentSizeFitter>();
        _originalPosition = _unitRectTransform.anchoredPosition;
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
        _moralText.text = newUnitStats._currentMoral.ToString();
        _powerText.text = CalculateSidePower(newUnitStats, newUnitStats._dice.GetCurrentSide()).ToString();
        _trigger.SetUnitBattleTooltip(newUnitStats);
    }

    public void SetupEnemy(NewUnitStats newUnitStats)
    {
        IsEnemy = true;
        int randSide = Random.Range(0, 6);
        DiceSide diceSide = newUnitStats._dice._diceConfig.sides[randSide];
        newUnitStats._dice.SetCurrentSide(diceSide);
        SetupCommon(newUnitStats);
        _powerText.text = newUnitStats._dice.GetCurrentSide().power.ToString();
        _trigger.SetUnitBattleTooltip(newUnitStats);
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
        }

        SetSelectionState(!IsSelected);
        SFXManager.Instance.PlayUISound(UISoundsEnum.UnitSelected);
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

            _moveTween?.Kill();

            Vector2 currentPosition = _unitRectTransform.anchoredPosition;
            // �������� ��������� layout ��� ��������
            if (selected)
            {
                _layoutWasEnabled = _parentLayoutGroup != null && _parentLayoutGroup.enabled;
                if (_parentLayoutGroup != null) _parentLayoutGroup.enabled = false;
                if (_parentFitter != null) _parentFitter.enabled = false;
                if (_layoutElement != null) _layoutElement.ignoreLayout = true;
            }

            float targetX = selected ?
                currentPosition.x + _selectedOffset.x :
                currentPosition.x - _selectedOffset.x;

            _moveTween = _unitRectTransform.DOAnchorPosX(targetX, 0.3f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => {
                    if (!selected)
                    {
                        // ��������������� layout
                        if (_layoutElement != null) _layoutElement.ignoreLayout = false;
                        if (_parentLayoutGroup != null && _layoutWasEnabled)
                            _parentLayoutGroup.enabled = true;
                        if (_parentFitter != null && _layoutWasEnabled)
                            _parentFitter.enabled = true;

                        // ������������� ��������� layout
                        LayoutRebuilder.MarkLayoutForRebuild(transform.parent as RectTransform);
                    }
                });

            _selectionIndicator?.SetActive(selected);
        }
    }


    public void RefreshUnitUI()
    {
        if (UnitData == null) return;

        _healthText.text = $"{UnitData._current_health}/{UnitData._health}";

        if (!IsEnemy)
        {
            _moralText.text = UnitData._currentMoral.ToString();
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

            // ���������� ������� ��� ����
            _defenseText.color = UnitData._current_defense >= UnitData._current_defense ?
                Color.cyan : // ������������ ���
                new Color(0.5f, 0.8f, 1f); // ������� ���
        }
        else
        {
            _defenseText.gameObject.SetActive(false);
        }
    }
    private void UpdateHealthVisuals()
    {
        float healthPercentage = (float)UnitData._current_health / UnitData._health;

        // ������ ���� ������ �������� � ����������� �� % ��������
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
        if (IsSelected)
        {
            SetSelectionState(false);
        }
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

    public void TakeDamage(int damage, BattleUnit attacker = null, bool isShieldEffect = true)
    {
        int originalDamage = damage;
        if (UnitData._current_defense > 0 && isShieldEffect == true)
        {
            int defenseReduction = Mathf.Min(UnitData._current_defense, damage);
            UnitData._current_defense -= defenseReduction;
            damage -= defenseReduction;
        }
        if (damage > 0)
        {
            UnitData._current_health = Mathf.Max(0, UnitData._current_health - damage);
        }
        if (UnitData._current_health != 0)
        {
            if (!IsEnemy)
            {
                SFXManager.Instance.PlaySoundSM(ActionType.Hit);
            }
            else
            {
                SFXManager.Instance.PlaySoundOrk(ActionType.Hit);
            }
        }
    }

    public void Heal(int amount)
    {
        int oldHealth = UnitData._current_health;
        UnitData._current_health = Mathf.Min(UnitData._current_health + amount, UnitData._health);
    }

    public void ModifyMorale(int amount)
    {
        UnitData._currentMoral += amount;
    }
    public void TakeMoraleDamage(int amount)
    {
        UnitData._currentMoral -= amount;
    }


    public void Die()
    {
        if (BattleController.Instance.UnitsObj.Contains(this))
        {
            BattleController.Instance.UnitsObj.Remove(this);
            TeamMNG.Instance.RemoveUnitFromPlayer(UnitData._ID);

            foreach (var unit in BattleController.Instance.UnitsObj)
            {
                unit.UnitData._currentMoral = Mathf.Max(0, unit.UnitData._currentMoral - 1);
            }
        }
        else if (BattleController.Instance.EnemiesObj.Contains(this))
        {
            if (Arrow != null)
                Arrow.gameObject.SetActive(false);

            BattleController.Instance.EnemiesObj.Remove(this);

            foreach (var unit in BattleController.Instance.UnitsObj)
            {
                unit.UnitData._currentMoral = Mathf.Max(0, unit.UnitData._currentMoral + 1);
            }
        }
        var intentions = BattleEnemyAI.Instance.EnemyIntentions;
        var toRemove = new List<BattleUnit>();

        foreach (var pair in intentions)
        {
            if (pair.Key == this || pair.Value == this)
                toRemove.Add(pair.Key);
        }

        foreach (var unit in toRemove)
        {
            intentions.Remove(unit);
        }

        BattleActionManager.Instance.PlayDeathAnimation(this);
    }

}