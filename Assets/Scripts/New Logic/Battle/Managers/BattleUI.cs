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
            RectTransform enemyPoint = enemyObj.LinePoint;
            RectTransform targetPoint = GetTargetObject(enemyIntentions, enemyObj).LinePoint;
            Debug.Log(enemyPoint.position + " " + targetPoint.position);
            Vector2 screenPosEnemy = RectTransformUtility.WorldToScreenPoint(cam, enemyPoint.position);
            Vector2 screenPosTarget = RectTransformUtility.WorldToScreenPoint(cam, targetPoint.position);
            Debug.Log(screenPosEnemy + " " + screenPosTarget);
            Vector2 direction = screenPosTarget - screenPosEnemy;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            enemyObj.Arrow.localRotation = Quaternion.Euler(0, 0, angle); 
            Vector2 length = -screenPosEnemy + screenPosTarget;
            Debug.Log(length.magnitude);
            //enemyObj.Arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(length.magnitude, 5f);
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
        GameWindowController.Instance.SetupWinBattleInfo(BattleController.Instance.CurrentBattleConfig);
        GameWindowController.Instance.CallPanel(1);
    }
    public void HideVictoryScreen() 
    {
        MenuMNG.Instance.ChangeVisibilityOfDownPanel();
        GlobalWindowController.Instance.GoBack();
    }

    public void ShowDefeatScreen() { }
    public void HideDefeatScreen() 
    {
        MenuMNG.Instance.ChangeVisibilityOfDownPanel();
        GlobalWindowController.Instance.GoBack();
    }
}