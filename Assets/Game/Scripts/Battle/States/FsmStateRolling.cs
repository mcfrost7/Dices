using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FsmStateRolling : FsmState
{
    private bool _startBattleSoundPlayed = false;
    public FsmStateRolling(FSM fsm) : base(fsm) { }
    public override void Enter()
    {
        BattleUI.Instance.StartBattleUISetup();
        BattleActionManager.Instance.ShowStateText("������:\n������ �����...");
        BattleDiceManager.Instance.OnAllRollsComplete += OnRollsComplete;

        BattleUI.Instance.ChangeMaxRerollText(RerollCalculator.CalculateRerolls(BattleController.Instance.PlayerUnits));
        BattleDiceManager.Instance.AllowMultipleSelections = false;

        BattleController.Instance.StartCoroutine(WaitForTutorialAndRoll());
    }

    private IEnumerator WaitForTutorialAndRoll()
    {
        yield return new WaitUntil(() => !GameDataMNG.Instance.Tutorial.isActive);
        BattleDiceManager.Instance.RollAllDice();
        if (!_startBattleSoundPlayed)
        {
            SFXManager.Instance.PlayStartBattleSound(GameDataMNG.Instance.MapGenerator.SelectedLocationConfig.enemyType);
            _startBattleSoundPlayed = true;
        }
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