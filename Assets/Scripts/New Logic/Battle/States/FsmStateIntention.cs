using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FsmStateIntention : FsmState
{
    private float _stateTimer = 0f;
    private bool _intentionsSelected = false;

    public FsmStateIntention(FSM fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Entering Intention state");
        _stateTimer = 0f;
        _intentionsSelected = false;

        // Enable UI for intention selection
        BattleUI.Instance.ShowIntentionPanel();

        // For each player unit, show intention options
        // This depends on your specific implementation
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

        // When intention phase is complete, move to next state
        if (_intentionsSelected)
        {
            Fsm.SetState<FsmStateRolling>();
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Intention state");
        // Hide intention UI
        BattleUI.Instance.HideIntentionPanel();
    }

    // Called by UI when player confirms intentions
    public void OnIntentionsConfirmed()
    {
        _intentionsSelected = true;
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