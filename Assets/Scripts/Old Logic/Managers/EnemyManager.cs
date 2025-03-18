using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    private List<UnitStats> enemies = new List<UnitStats>();
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private TypesInfo typesInfo;

    private void Awake()
    {
        gameObject.SetActive(false);
    }
    public void CreateEnemies()
    {
        int localDiff = DifficultyScaler();

        enemies.Clear();

        for (int i = 0; i < localDiff; i++)
        {
            int randomHealth = Random.Range(1, localDiff);
            TypesInfo.Type randomType = typesInfo.types[Random.Range(0, typesInfo.types.Length)];
            enemies.Add(new UnitStats(randomHealth, randomType));
        }
    }

    public int DifficultyScaler()
    {
        return GameManager.Instance.Player.Difficulty < 5 ? GameManager.Instance.Player.Difficulty++ : GameManager.Instance.Player.Difficulty;
    }

    public List<UnitStats> Enemies
    {
        get => enemies;
        set => enemies = value;
    }

    public GameObject EnemyPrefab
    {
        get => enemyPrefab;
        set => enemyPrefab = value;
    }

    public void FindTarget(List<GameObject> unitsObj, List<GameObject> enemiesObj)
    {
        List<Unit> unitList = new List<Unit>();
        List<UnitStats> enemyList = new List<UnitStats>();
        foreach (GameObject unit in unitsObj)
        {
            unitList.Add(unit.GetComponent<Unit>());
        }
        foreach (GameObject unit in enemiesObj)
        {
            enemyList.Add(unit.GetComponent<Unit>().UnitStats);
        }

        ActionResolver actionResolver = new ActionResolver();
        List<Unit> targets = new List<Unit>();
        foreach (UnitStats enemyUnit in enemyList)
        {
            enemyUnit.Target = actionResolver.ChooseUnitToAttack(unitList);
            Debug.Log("Цель с именем " + enemyUnit.Target.UnitStats.Name + " для " + enemyUnit.Name+ " .");
        }
    }
}
