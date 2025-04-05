using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FsmStateReroll : FsmState
{
    private float _stateTimer = 0f;


    public FsmStateReroll(FSM fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Entering Reroll state");
        _stateTimer = 0f;
        BattleRerolls.Instance.InitRerolls();
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

    }

    public override void Exit()
    {
        Debug.Log("Exiting Reroll state");
        // Hide reroll UI
        //BattleUI.Instance.HideRerollPanel();

        // Execute any pending rerolls
       // BattleDiceManager.Instance.ExecuteRerolls();
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