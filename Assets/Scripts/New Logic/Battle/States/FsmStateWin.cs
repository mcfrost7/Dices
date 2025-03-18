using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmStateWin : FsmState
{
    private float _stateTimer = 0f;
    private float _victoryScreenDuration = 3f;

    public FsmStateWin(FSM fsm) : base(fsm) { }

    public override void Enter()
    {
        Debug.Log("Victory! Player has won the battle.");
        _stateTimer = 0f;

        // Show victory screen
        BattleUI.Instance.ShowVictoryScreen();

        // Trigger win callbacks
        BattleController.Instance.OnBattleWin();
    }

    public override void Update()
    {
        _stateTimer += Time.deltaTime;

        // Automatically proceed after delay or player input
        if (_stateTimer >= _victoryScreenDuration || Input.GetMouseButtonDown(0))
        {
            // Return to map or next step
            //GlobalWindowController.Instance.HideBattle();
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting victory state");
        BattleUI.Instance.HideVictoryScreen();
    }
}