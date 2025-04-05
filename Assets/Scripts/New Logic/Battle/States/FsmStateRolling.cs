using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmStateRolling : FsmState
{
    private float _stateTimer = 0f;
    public FsmStateRolling(FSM fsm) : base(fsm) { }
    public override void Enter()
    {
        Debug.Log("Entering Rolling state");
        _stateTimer = 0f;
        BattleDiceManager.Instance.OnAllRollsComplete += OnRollsComplete;
        BattleDiceManager.Instance.RollAllDice();
        BattleUI.Instance.ChangeMaxRerollText(RerollCalculator.CalculateRerolls(BattleController.Instance.PlayerUnits));
        
    }

    public override void Exit()
    {
        BattleDiceManager.Instance.OnAllRollsComplete -= OnRollsComplete;
    }

    private void OnRollsComplete()
    {
        Fsm.SetState<FsmStateReroll>();
    }

    public override void Update()
    {
        _stateTimer += Time.deltaTime;

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

    private bool CheckAllPlayersDead()
    {
        if (BattleController.Instance.UnitsObj.Count == 0)
            return true;
        return false; // Replace with actual logic
    }

    private bool CheckAllEnemiesDead()
    {
        if (BattleController.Instance.EnemiesObj.Count == 0)
            return true;
        return false; // Replace with actual logic
    }
}