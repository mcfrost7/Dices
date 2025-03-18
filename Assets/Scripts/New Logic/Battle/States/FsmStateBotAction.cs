using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmStateBotAction : FsmState
{
    private float _stateTimer = 0f;
    private float _actionDuration = 2f; // Duration of enemy actions

    public FsmStateBotAction(FSM fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Entering Bot Action state");
        _stateTimer = 0f;

        // Trigger enemy/bot actions
        BattleEnemyAI.Instance.ExecuteActions();
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

        // When bot action is complete, cycle back to Intention state
        if (_stateTimer >= _actionDuration || BattleEnemyAI.Instance.AreActionsComplete())
        {
            Fsm.SetState<FsmStateIntention>();
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Bot Action state");
        // Reset for next turn
        BattleTurnManager.Instance.EndTurn();
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