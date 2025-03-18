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

        // Show action UI
        BattleUI.Instance.ShowActionPanel();

        // Calculate available actions based on dice results
        BattleActionManager.Instance.CalculateAvailableActions();
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
        // Hide action UI
       // BattleUI.Instance.HideActionPanel();
    }

    // Called by UI when player completes their actions
    public void OnActionsComplete()
    {
        _actionsComplete = true;
    }

    private bool CheckAllPlayersDead()
    {
        // Check if all player units are defeated
        return false; // Replace with actual logic
    }

    private bool CheckAllEnemiesDead()
    {
        // Check if all enemy units are defeated
        return false; // Replace with actual logic
    }
}