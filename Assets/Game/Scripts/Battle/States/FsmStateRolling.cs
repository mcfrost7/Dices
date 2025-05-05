using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmStateRolling : FsmState
{
    private float _stateTimer = 0f;
    public FsmStateRolling(FSM fsm) : base(fsm) { }
    public override void Enter()
    {
        BattleUI.Instance.StartBattleUISetup();
        BattleActionManager.Instance.ShowStateText("Стадия:\nБросок кубов...");
        _stateTimer = 0f;
        BattleDiceManager.Instance.OnAllRollsComplete += OnRollsComplete;
        BattleDiceManager.Instance.RollAllDice();
        BattleUI.Instance.ChangeMaxRerollText(RerollCalculator.CalculateRerolls(BattleController.Instance.PlayerUnits));
        BattleDiceManager.Instance.AllowMultipleSelections = false;

    }

    public override void Exit()
    {
        BattleDiceManager.Instance.OnAllRollsComplete -= OnRollsComplete;
    }

    private void OnRollsComplete()
    {
        Fsm.SetState<FsmStateIntention>();
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
        return false; 
    }

    private bool CheckAllEnemiesDead()
    {
        if (BattleController.Instance.EnemiesObj.Count == 0)
            return true;
        return false; 
    }
}