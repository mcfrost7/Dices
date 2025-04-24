using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmStateBotAction : FsmState
{


    public FsmStateBotAction(FSM fsm) : base(fsm) { }

    public override void Enter()
    {
        BattleActionManager.Instance.ShowStateText("Стадия:\nХод врага");

        BattleEnemyAI.Instance.ExecuteActions();
    }

    public override void Update()
    {


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
        if (BattleEnemyAI.Instance.AreActionsComplete())
        {
            Fsm.SetState<FsmStateRolling>();
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