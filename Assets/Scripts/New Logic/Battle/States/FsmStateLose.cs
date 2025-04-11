using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmStateLose : FsmState
{

    public FsmStateLose(FSM fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Defeat! Player has lost the battle.");
        BattleUI.Instance.ShowDefeatScreen();
        BattleController.Instance.OnBattleLose();
        Fsm.StopMachine();
    }

    public override void Exit()
    {
        Debug.Log("Exiting defeat state");
        BattleUI.Instance.HideDefeatScreen();
    }
}