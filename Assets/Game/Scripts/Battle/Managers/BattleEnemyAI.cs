using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleEnemyAI : MonoBehaviour
{
    public static BattleEnemyAI Instance { get; private set; }
    public Dictionary<BattleUnit, BattleUnit> EnemyIntentions { get => enemyIntentions; set => enemyIntentions = value; }
    private bool endAction = false;

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

    private Dictionary<BattleUnit, BattleUnit> enemyIntentions = new Dictionary<BattleUnit, BattleUnit>();

    public bool CreateIntention()
    {
        EnemyIntentions = new Dictionary<BattleUnit, BattleUnit>();
        foreach (var enemy in BattleController.Instance.EnemiesObj)
        {
            DiceSide currentSide = enemy.UnitData._dice.GetCurrentSide();
            BattleUnit target;
            if (currentSide != null && currentSide.ActionSide == ActionSide.Ally)
            {
                target = ChooseWeakestAlly(enemy, BattleController.Instance.EnemiesObj);
            }
            else
            {
                target = ChooseTarget(enemy, BattleController.Instance.UnitsObj);
            }

            if (target != null)
            {
                EnemyIntentions[enemy] = target;
            }
        }

        Debug.Log($"CreateIntention: Завершено создание намерений, всего: {EnemyIntentions.Count}");
        return EnemyIntentions.Count > 0;
    }

    private BattleUnit ChooseWeakestAlly(BattleUnit source, List<BattleUnit> alliesUnits)
    {
        if (alliesUnits.Count <= 1)
        {
            return source;
        }
        List<BattleUnit> possibleTargets = alliesUnits.Where(u => u != source).ToList();
        if (possibleTargets.Count == 0)
        {
            return source;
        }
        BattleUnit weakestAlly = null;
        float minHealthPercentage = float.MaxValue;

        foreach (var ally in possibleTargets)
        {
            float healthPercentage = (float)ally.UnitData._current_health / ally.UnitData._current_health * 100f;
            bool hasShield = ally.UnitData._current_defense != 0 ? true: false;
            if (healthPercentage < minHealthPercentage ||
                (healthPercentage == minHealthPercentage && !hasShield && weakestAlly != null && weakestAlly.UnitData._current_defense>0))
            {
                minHealthPercentage = healthPercentage;
                weakestAlly = ally;
            }
        }
        return weakestAlly ?? source;
    }

    private BattleUnit ChooseTarget(BattleUnit enemy, List<BattleUnit> playerUnits)
    {
        ConsidirationController considiration = new ConsidirationController();
        Dictionary<BattleUnit, float> unitScores = considiration.EvaluatePlayerUnits(playerUnits);
        var target = unitScores.OrderByDescending(u => u.Value).First().Key;
        return target;
    }

    public void ExecuteActions()
    {
        StartCoroutine(ExecuteActionsCoroutine());
    }

    private IEnumerator ExecuteActionsCoroutine()
    {
        endAction = false;

        var enemiesCopy = BattleController.Instance.EnemiesObj.ToList(); 

        foreach (var source in enemiesCopy)
        {
            if (EnemyIntentions.TryGetValue(source, out var target) && target != null)
            {
                source.Arrow.gameObject.SetActive(false);
                BattleActionManager.Instance.ExecuteAction(source, target);
                yield return new WaitForSeconds(1.5f);
            }
        }

        endAction = true;
    }


    public bool AreActionsComplete() { return endAction; }
}

