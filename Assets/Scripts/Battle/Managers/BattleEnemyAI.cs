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
            BattleUnit target = ChooseTarget(enemy, BattleController.Instance.UnitsObj);
            if (target != null)
            {
                EnemyIntentions[enemy] = target; // Записываем цель в словарь
            }
        }

        Debug.Log($"CreateIntention: Завершено создание намерений, всего: {EnemyIntentions.Count}");
        return EnemyIntentions.Count > 0;
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
        foreach (var source in BattleController.Instance.EnemiesObj)
        {
            BattleUnit target = EnemyIntentions.TryGetValue(source, out var targetIn) ? targetIn : null;
            if (target != null)
            {
                BattleActionManager.Instance.ExecuteAction(source, target);
                source.Arrow.gameObject.SetActive(false);  
                yield return new WaitForSeconds(1f);
            }
        }
        endAction = true;
    }
    public bool AreActionsComplete() { return endAction; }
}
