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
    public void ShowIntentionDelayed(Dictionary<NewUnitStats, NewUnitStats> enemyIntentions)
    {
        StartCoroutine(DelayedShow(enemyIntentions));
    }

    private IEnumerator DelayedShow(Dictionary<NewUnitStats, NewUnitStats> enemyIntentions)
    {
        yield return null; // подождать 1 кадр
        ShowIntention(enemyIntentions);
    }
    public void ShowIntention(Dictionary<NewUnitStats, NewUnitStats> enemyIntentions)
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
    private static BattleUnit GetTargetObject(Dictionary<NewUnitStats, NewUnitStats> enemyIntentions, BattleUnit enemyObj)
    {
        NewUnitStats enemyUnitStats = enemyObj.UnitData;
        if (enemyIntentions.TryGetValue(enemyUnitStats, out NewUnitStats targetUnitStats))
        {
            BattleUnit enemyTarget = BattleController.Instance.UnitsObj
                 .Find(unit => unit.GetComponent<BattleUnit>().UnitData == targetUnitStats)
     ?.GetComponent<BattleUnit>();
            return enemyTarget;
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

    public void ShowVictoryScreen() { }
    public void HideVictoryScreen() { }

    public void ShowDefeatScreen() { }
    public void HideDefeatScreen() { }
}