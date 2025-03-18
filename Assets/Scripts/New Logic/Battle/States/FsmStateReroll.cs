using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FsmStateReroll : FsmState
{
    private float _stateTimer = 0f;
    private float _maxRerollTime = 10f; // Max time allowed for reroll decision
    private bool _rerollsConfirmed = false;

    public FsmStateReroll(FSM fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Entering Reroll state");
        _stateTimer = 0f;
        _rerollsConfirmed = false;

        // Show reroll UI
        BattleUI.Instance.ShowRerollPanel();
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

        // Move to next state when rerolls are confirmed or time runs out
        if (_rerollsConfirmed || _stateTimer >= _maxRerollTime || BattleUI.Instance.AreRerollsConfirmed())
        {
            Fsm.SetState<FsmStateAction>();
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

    // Called by UI when player confirms rerolls
    public void OnRerollsConfirmed()
    {
        _rerollsConfirmed = true;
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