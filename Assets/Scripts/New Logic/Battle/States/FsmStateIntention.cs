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
        BattleUI.Instance.StartBattleUISetup();
        if (BattleEnemyAI.Instance.CreateIntention())
        {
            BattleUI.Instance.ShowIntentionDelayed(BattleEnemyAI.Instance.EnemyIntentions);
            OnIntentionsConfirmed();
        }

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

        if (_intentionsSelected)
        {
            Fsm.SetState<FsmStateRolling>();
        }
    }

    public override void Exit()
    {
        Debug.Log("Exiting Intention state");
    }

    public void OnIntentionsConfirmed()
    {
        _intentionsSelected = true;
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