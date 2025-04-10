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
    public bool IsWaitingForTarget { get => _isWaitingForTarget; }

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
        // Не позволяем выбирать использованного юнита как исполнителя
        if (unit.IsUsed)
        {
            ShowActionFeedback("Этот юнит уже действовал в этом ходу");
            return;
        }

        _selectedUnit = unit;
        _isWaitingForTarget = true;

        ShowActionFeedback("Выберите цель для действия");
    }

    public void SetTargetUnit(BattleUnit unit)
    {
        if (!_isWaitingForTarget || _selectedUnit == null)
            return;

        _targetUnit = unit;

        // Проверяем валидность цели
        if (IsValidTarget(_selectedUnit, _targetUnit))
        {
            ExecuteAction(_selectedUnit, _targetUnit);
            _isWaitingForTarget = false;
            HideActionFeedback();
        }
        else
        {
            // Показываем сообщение о неправильной цели
            ShowActionFeedback("Недопустимая цель для этого действия!");

            // Используем существующий метод для сброса всех выделений
            DeselectUnit();
        }
    }

    public void ExecuteAction(BattleUnit source, BattleUnit target)
    {
        if (source == null || target == null)
            return;

        DiceSide currentSide = source.UnitData._dice.GetCurrentSide();
        int power = source.CalculateSidePowerWithBuffs(source.UnitData, currentSide);
        int duration = source.UnitData._dice.GetCurrentSide().duration;

        // Получаем нужное действие и выполняем его
        IBattleAction action = BattleActionFactory.Instance.GetAction(currentSide.actionType);
        action.Execute(source, target, power,duration);

        source.DisableUnitAfterAction();
        source.RefreshUnitUI();
        target.RefreshUnitUI();
        DeselectUnit();
        CheckForDefeat(target);
    }

    private bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        if (source == null || target == null || source.UnitData == null || target.UnitData == null)
            return false;

        ActionType actionType = source.UnitData._dice.GetCurrentSide().actionType;
        IBattleAction action = BattleActionFactory.Instance.GetAction(actionType);
        return action.IsValidTarget(source, target);
    }

    private void CheckForDefeat(BattleUnit target)
    {
        if (target.UnitData._current_health <= 0)
        {
            // Remove defeated unit
            if (BattleController.Instance.UnitsObj.Contains(target))
            {
                BattleController.Instance.UnitsObj.Remove(target);
                TeamMNG.Instance.RemoveUnitFromPlayer(target.UnitData._ID);
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
        foreach (var unit in BattleController.Instance.EnemiesObj)
        {
            unit.EnableUnitForNewTurn();
        }
    }

    public void DeselectUnit()
    {
        if (_selectedUnit != null)
        {
            _selectedUnit.SetSelectionState(false);
            _selectedUnit = null;
        }
        if (_targetUnit != null)
        {
            _targetUnit.SetSelectionState(false);
            _targetUnit = null;
        }
        _isWaitingForTarget = false;
        HideActionFeedback();
    }
}