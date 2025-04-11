using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmStateLose : FsmState
{
    private float _stateTimer = 0f;
    private float _defeatScreenDuration = 3f;

    public FsmStateLose(FSM fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Defeat! Player has lost the battle.");
        _stateTimer = 0f;
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