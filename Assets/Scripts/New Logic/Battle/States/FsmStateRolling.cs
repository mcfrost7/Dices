using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmStateRolling : FsmState
{
    private float _rollingDuration = 1.5f; // Duration of rolling animation
    private float _stateTimer = 0f;

    public FsmStateRolling(FSM fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Entering Rolling state");
        _stateTimer = 0f;

        // Start dice rolling animation
        BattleUI.Instance.ShowDiceRolling();

        // Generate initial dice results for each unit with intentions
       // BattleDiceManager.Instance.RollAllDice();
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

        // When rolling animation is complete, move to reroll state
        if (_stateTimer >= _rollingDuration)
        {
            Fsm.SetState<FsmStateReroll>();
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Rolling state");
        // Show final dice results
        BattleUI.Instance.ShowDiceResults();
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