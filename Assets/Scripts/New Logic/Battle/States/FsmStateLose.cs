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

        // Show defeat screen
        BattleUI.Instance.ShowDefeatScreen();

        // Trigger lose callbacks
        BattleController.Instance.OnBattleLose();
    }

    public override void Update()
    {
        _stateTimer += Time.deltaTime;

        // Automatically proceed after delay or player input
        if (_stateTimer >= _defeatScreenDuration || Input.GetMouseButtonDown(0))
        {
            // Return to map or game over screen
            //GlobalWindowController.Instance.HideBattle();
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting defeat state");
        //BattleUI.Instance.HideDefeatScreen();
    }
}