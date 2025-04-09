using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class BattleActionManager : MonoBehaviour
{
    [SerializeField] private Button _endAction;
    [SerializeField] private Button _deselect;
    [SerializeField] private TextMeshProUGUI _actionFeedbackText;

    private BattleUnit _selectedUnit;
    private BattleUnit _targetUnit;
    private bool _isWaitingForTarget = false;

    public event System.Action ActionComplete;
    public static BattleActionManager Instance { get; private set; }
    public Button EndAction { get => _endAction; set => _endAction = value; }

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

        _endAction.onClick.AddListener(EndActionButton);
        _deselect.onClick.AddListener(DeselectUnit);

        if (_actionFeedbackText != null)
        {
            _actionFeedbackText.gameObject.SetActive(false);
        }
    }

    public void EnableEnemyUnitSelection()
    {
        foreach (BattleUnit unit in BattleController.Instance.EnemiesObj)
        {
            if (unit != null && unit.ActionTrigger != null)
            {
                unit.ActionTrigger.enabled = true;
            }
        }
    }

    public void SetSelectedUnit(BattleUnit unit)
    {
        _selectedUnit = unit;
        _isWaitingForTarget = true;

        // Enable target selection UI feedback
        ShowActionFeedback("Select a target for this action");
    }

    public void SetTargetUnit(BattleUnit unit)
    {
        if (!_isWaitingForTarget || _selectedUnit == null)
            return;

        _targetUnit = unit;

        // Check if action is valid for this target
        if (IsValidTarget(_selectedUnit, _targetUnit))
        {
            ExecuteAction(_selectedUnit, _targetUnit);
            _isWaitingForTarget = false;
            HideActionFeedback();
        }
        else
        {
            // Show feedback that target is invalid
            ShowActionFeedback("Invalid target for this action type!");
        }
    }

    private bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        if (source == null || target == null || source.UnitData == null || target.UnitData == null)
            return false;

        ActionSide actionType = source.UnitData._dice.GetCurrentSide().ActionSide;

        // Check if source is player unit
        bool isSourcePlayer = BattleController.Instance.UnitsObj.Contains(source);
        bool isTargetPlayer = BattleController.Instance.UnitsObj.Contains(target);

        switch (actionType)
        {
            case ActionSide.Enemy:
                // Can only target enemies
                return isSourcePlayer != isTargetPlayer;

            case ActionSide.Ally:
                // Can only target allies or self
                return isSourcePlayer == isTargetPlayer;

            case ActionSide.None:
                // Can target anyone
                return true;

            default:
                return false;
        }
    }

    private void ExecuteAction(BattleUnit source, BattleUnit target)
    {
        if (source == null || target == null)
            return;

        DiceSide currentSide = source.UnitData._dice.GetCurrentSide();
        int power = source.CalculateSidePowerWithBuffs(source.UnitData, currentSide);

        // Apply the action effects based on the dice side
        switch (currentSide.actionType)
        {
            case ActionType.Attack:
                // Deal damage to enemy
                target.UnitData._current_health -= power;
                break;

            case ActionType.Heal:
                // Heal ally or self
                target.UnitData._current_health = Mathf.Min(
                    target.UnitData._current_health + power,
                    target.UnitData._health
                );
                break;

            case ActionType.None:
                // Generic action - default to damage
                target.UnitData._current_health -= power;
                break;
        }

        source.DisableUnitAfterAction();
        source.RefreshUnitUI();
        target.RefreshUnitUI();

        // Clear selection after action
        DeselectUnit();

        // Check if unit is defeated
        CheckForDefeat(target);
    }

    private void CheckForDefeat(BattleUnit target)
    {
        if (target.UnitData._current_health <= 0)
        {
            // Remove defeated unit
            if (BattleController.Instance.UnitsObj.Contains(target))
            {
                BattleController.Instance.UnitsObj.Remove(target);
            }
            else if (BattleController.Instance.EnemiesObj.Contains(target))
            {
                BattleController.Instance.EnemiesObj.Remove(target);
                BattleUI.Instance.ShowIntentionDelayed(BattleEnemyAI.Instance.EnemyIntentions);
            }

            // Destroy the game object
            Destroy(target.gameObject);
        }
    }

    public void ShowActionFeedback(string message)
    {
        if (_actionFeedbackText != null)
        {
            _actionFeedbackText.gameObject.SetActive(true);
            _actionFeedbackText.text = message;
        }
    }

    public void HideActionFeedback()
    {
        if (_actionFeedbackText != null)
        {
            _actionFeedbackText.gameObject.SetActive(false);
        }
    }

    public void EndActionButton()
    {
        ActionComplete?.Invoke();
        _endAction.gameObject.SetActive(false);
        BattleRerolls.Instance.EndRerolls.gameObject.SetActive(true);
        BattleRerolls.Instance.EndRerolls.enabled = false;
        foreach (var unit in BattleController.Instance.UnitsObj)
        {
            unit.EnableUnitForNewTurn();
        }
    }

    public void DeselectUnit()
    {
        if (_selectedUnit != null)
        {
            _selectedUnit = null;
        }
        _targetUnit = null;
        _isWaitingForTarget = false;
        HideActionFeedback();
    }
}