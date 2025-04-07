using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmStateAction : FsmState
{
    private float _stateTimer = 0f;
    private bool _actionsComplete = false;

    public FsmStateAction(FSM fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Entering Action state");
        _stateTimer = 0f;
        _actionsComplete = false;
        BattleActionManager.Instance.ActionComplete += OnActionsComplete;
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
        Debug.Log("Exiting Action state");
        BattleActionManager.Instance.ActionComplete -= OnActionsComplete;
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