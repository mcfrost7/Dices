using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmStateAction : FsmState
{
    private float _stateTimer = 0f;
    private bool _actionsComplete = false;
    private BattleUnit _currentSelectedUnit = null;
    public FsmStateAction(FSM fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Entering Action state");
        _stateTimer = 0f;
        _actionsComplete = false;

        BattleActionManager.Instance.ActionComplete += OnActionsComplete;
        BattleActionManager.Instance.EndAction.gameObject.SetActive(true);
        BattleDiceManager.Instance.EnablePlayerUnitSelections();
        BattleDiceManager.Instance.UnitSelected += OnUnitSelected;
    }

    public override void Update()
    {
        _stateTimer += Time.deltaTime;

        // Check for win/lose conditions
        if (CheckAllPlayersDead())
        {
            Fsm.SetState<FsmStateLose>();
            return;
        }

        if (CheckAllEnemiesDead())
        {
            Fsm.SetState<FsmStateWin>();
            return;
        }

        // Move to next state when actions are complete
        if (_actionsComplete /*|| BattleUI.Instance.AreActionsComplete()*/)
        {
            Fsm.SetState<FsmStateBotAction>();
        }
    }

    public override void Exit()
    {
        BattleActionManager.Instance.ActionComplete -= OnActionsComplete;
        BattleDiceManager.Instance.UnitSelected -= OnUnitSelected;
        BattleDiceManager.Instance.AllowMultipleSelections = true; // возвращаем мультивыделение

        if (_currentSelectedUnit != null)
        {
            _currentSelectedUnit.SetSelectionState(false);
            _currentSelectedUnit = null;
        }
    }

    private void OnUnitSelected(BattleUnit unit)
    {
        // If we already had a selected unit, deselect it
        if (_currentSelectedUnit != null && _currentSelectedUnit != unit)
        {
            _currentSelectedUnit.SetSelectionState(false);
        }
        
        // Update our reference to the currently selected unit
        _currentSelectedUnit = unit.IsSelected ? unit : null;
        
        if (_currentSelectedUnit != null)
        {
            // A unit is selected, set it as the active unit in BattleActionManager
            BattleActionManager.Instance.SetSelectedUnit(_currentSelectedUnit);
        }
        else
        {
            // No unit is selected
            BattleActionManager.Instance.DeselectUnit();
        }
    }

    // Called by UI when player completes their actions
    public void OnActionsComplete()
    {
        _actionsComplete = true;
    }

    private bool CheckAllPlayersDead()
    {
        if (BattleController.Instance.UnitsObj.Count == 0)
            return true;
        return false;
    }

    private bool CheckAllEnemiesDead()
    {
        if (BattleController.Instance.EnemiesObj.Count == 0)
            return true;
        return false;
    }
}