using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _currentRolls;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }


    public static BattleUI Instance { get; private set; }


    public void StartBattleUISetup()
    {
        BattleActionManager.Instance.HideActionFeedback();
        BattleActionManager.Instance.EndActionButton();


    }
    public void ShowIntentionDelayed(Dictionary<BattleUnit, BattleUnit> enemyIntentions)
    {
        StartCoroutine(DelayedShow(enemyIntentions));
    }

    private IEnumerator DelayedShow(Dictionary<BattleUnit, BattleUnit> enemyIntentions)
    {
        yield return null; // подождать 1 кадр
        ShowIntention(enemyIntentions);
    }
    public void ShowIntention(Dictionary<BattleUnit, BattleUnit> enemyIntentions)
    {
        Camera cam = Camera.main;
        Canvas.ForceUpdateCanvases();
        foreach (var enemyObj in BattleController.Instance.EnemiesObj)
        {
            if (enemyObj.UnitData._dice.GetCurrentSide().actionType != ActionType.None)
            {
                enemyObj.Arrow.gameObject.SetActive(true);
                RectTransform enemyPoint = enemyObj.LinePoint;
                BattleUnit targetUnit = GetTargetObject(enemyIntentions, enemyObj);
                RectTransform targetPoint = targetUnit != null ? targetUnit.LinePoint : null;
                if (targetPoint != null)
                {
                    Vector2 screenPosEnemy = RectTransformUtility.WorldToScreenPoint(cam, enemyPoint.position);
                    Vector2 screenPosTarget = RectTransformUtility.WorldToScreenPoint(cam, targetPoint.position);
                    Vector2 direction = screenPosTarget - screenPosEnemy;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    enemyObj.Arrow.localRotation = Quaternion.Euler(0, 0, angle);
                    Vector2 length = -screenPosEnemy + screenPosTarget;
                }
                else
                {
                    enemyObj.Arrow.gameObject.SetActive(false);
                }
            }

        }
    }
    private static BattleUnit GetTargetObject(Dictionary<BattleUnit, BattleUnit> enemyIntentions, BattleUnit enemyObj)
    {
        if (enemyIntentions == null || enemyObj == null)
        {
            return null;
        }
        if (enemyIntentions.TryGetValue(enemyObj, out BattleUnit targetUnit))
        {
            if (targetUnit != null && BattleController.Instance.UnitsObj.Contains(targetUnit))
            {
                return targetUnit;
            }
        }

        return null;
    }


    public void ChangeMaxRerollText(int rerolls)
    {
        _currentRolls.text = rerolls.ToString() ;
    }

    public void ShowActionPanel() { }
    public void HideActionPanel() 
    {
        BattleActionManager.Instance.HideActionFeedback();
    }
    public bool AreActionsComplete() { return false; }

    public void ShowVictoryScreen()
    {
        if (BattleController.Instance.CurrentBattleConfig.tileType == TileType.BattleTile)
            GameWindowController.Instance.SetupWinBattleInfo(BattleController.Instance.CurrentBattleConfig);
        else
            GameWindowController.Instance.SetupWinBosseInfo(BattleController.Instance.CurrentBattleConfig);
        GameWindowController.Instance.CallPanel(1);

    }
    public void HideVictoryScreen() 
    {
        MenuMNG.Instance.SetActiveDownPanel();
    }

    public void ShowDefeatScreen()
    {
        GameWindowController.Instance.SetupDefeatBattleInfo();
        GameWindowController.Instance.CallPanel(1);
    }
    public void HideDefeatScreen() 
    {
        MenuMNG.Instance.SetActiveDownPanel();
    }
}