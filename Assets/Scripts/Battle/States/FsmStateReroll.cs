using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FsmStateReroll : FsmState
{
    private float _stateTimer = 0f;

    public FsmStateReroll(FSM fsm) : base(fsm) { }

    public override void Enter()
    {
        BattleActionManager.Instance.ShowStateText("Стадия:\nПереброска кубов");
        _stateTimer = 0f;
        BattleRerolls.Instance.InitRerolls();
        BattleUI.Instance.ShowIntentionDelayed(BattleEnemyAI.Instance.EnemyIntentions);
        BattleDiceManager.Instance.OnAllRerollsComplete += OnRerollsComplete;
        BattleRerolls.Instance.OnAllRerollsComplete += OnEndRerollsPressed;
        BattleDiceManager.Instance.AllowMultipleSelections = true;
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

    private void OnRerollsComplete()
    {
        Debug.Log("All rerolls completed in FSM state");
        BattleDiceManager.Instance.EnablePlayerUnitSelections();
    }

    private void OnEndRerollsPressed()
    {
        Debug.Log("End Rerolls button pressed, transitioning to Action state");
        Fsm.SetState<FsmStateAction>();
    }

    public override void Exit()
    {
        BattleRerolls.Instance.DeselectPlayerUnits();
        BattleDiceManager.Instance.OnAllRerollsComplete -= OnRerollsComplete;
        BattleRerolls.Instance.OnAllRerollsComplete -= OnEndRerollsPressed;
        Debug.Log("Exiting Reroll state");
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