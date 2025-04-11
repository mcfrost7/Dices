using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmStateWin : FsmState
{

    public FsmStateWin(FSM fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Victory! Player has won the battle.");
        BattleController.Instance.OnBattleWin();
        BattleUI.Instance.ShowVictoryScreen();
        Fsm.StopMachine();
    }

    public override void Exit()
    {
        Debug.Log("Exiting victory state");
        BattleUI.Instance.HideVictoryScreen();
    }
}