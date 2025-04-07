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
        BattleDiceManager.Instance.OnAllRerollsComplete += OnRerollsComplete;

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
        if (BattleRerolls.Instance.AvailableRerolls == 0 )
        {
            Fsm.SetState<FsmStateAction>();
        }
    }
    private void OnRerollsComplete()
    {
        Debug.Log("All rerolls completed in FSM state");

        // Здесь можно переходить в следующий стейт или оставить только для Enable UI и логики
        BattleDiceManager.Instance.EnablePlayerUnitSelections();
    }

    public override void Exit()
    {
        BattleDiceManager.Instance.OnAllRerollsComplete -= OnRerollsComplete;
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