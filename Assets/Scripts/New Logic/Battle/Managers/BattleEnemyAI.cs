using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleEnemyAI : MonoBehaviour
{
    public static BattleEnemyAI Instance { get; private set; }
    public Dictionary<NewUnitStats, NewUnitStats> EnemyIntentions { get => enemyIntentions; set => enemyIntentions = value; }

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

    private Dictionary<NewUnitStats, NewUnitStats> enemyIntentions = new Dictionary<NewUnitStats, NewUnitStats>();
    public bool CreateIntention()
    {
        foreach (var enemy in BattleController.Instance.EnemyUnits)
        {
            NewUnitStats target = ChooseTarget(enemy, BattleController.Instance.PlayerUnits);
            if (target != null)
            {
                EnemyIntentions[enemy] = target; // Записываем цель в словарь
            }
        }
        return true;
    }
    private NewUnitStats ChooseTarget(NewUnitStats enemy, List<NewUnitStats> playerUnits)
    {
        ConsidirationController considiration = new ConsidirationController();
        Dictionary<NewUnitStats, float> unitScores = considiration.EvaluatePlayerUnits(playerUnits);

        foreach (var unit in unitScores)
        {
            Debug.Log($"[AI Target Selection] Юнит: {unit.Key._name}, Стоимость: {unit.Value}");
        }

        // Выбираем цель с максимальной "стоимостью"
        var target = unitScores.OrderByDescending(u => u.Value).First().Key;

        Debug.Log($"[AI Target Choice] Враг {enemy._name} выбрал целью {target._name}");

        return target;
    }

    public void ExecuteActions() { }
    public bool AreActionsComplete() { return true; }
}
