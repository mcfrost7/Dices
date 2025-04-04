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
        // ��������, ��� ������� ���������� � ����
        if (EnemyIntentions == null)
        {
            EnemyIntentions = new Dictionary<NewUnitStats, NewUnitStats>();
        }
        else
        {
            EnemyIntentions = new Dictionary<NewUnitStats, NewUnitStats>();
        }

        // �������������� ��� ��� �������
        Debug.Log($"CreateIntention: ������ �������� ���������, �����������: {BattleController.Instance.EnemyUnits.Count}");

        foreach (var enemy in BattleController.Instance.EnemyUnits)
        {
            NewUnitStats target = ChooseTarget(enemy, BattleController.Instance.PlayerUnits);
            if (target != null)
            {
                EnemyIntentions[enemy] = target; // ���������� ���� � �������
            }
            else
            {
                Debug.LogWarning($"CreateIntention: ��������� {enemy._name} �� ���� ������� ����");
            }
        }

        Debug.Log($"CreateIntention: ��������� �������� ���������, �����: {EnemyIntentions.Count}");
        return EnemyIntentions.Count > 0;
    }
    private NewUnitStats ChooseTarget(NewUnitStats enemy, List<NewUnitStats> playerUnits)
    {
        ConsidirationController considiration = new ConsidirationController();
        Dictionary<NewUnitStats, float> unitScores = considiration.EvaluatePlayerUnits(playerUnits);

        foreach (var unit in unitScores)
        {
            Debug.Log($"[AI Target Selection] ����: {unit.Key._name}, ���������: {unit.Value}");
        }

        // �������� ���� � ������������ "����������"
        var target = unitScores.OrderByDescending(u => u.Value).First().Key;

        Debug.Log($"[AI Target Choice] ���� {enemy._name} ������ ����� {target._name}");

        return target;
    }

    public void ExecuteActions() { }
    public bool AreActionsComplete() { return true; }
}
