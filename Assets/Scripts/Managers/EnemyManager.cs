using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    private List<UnitStats> enemies = new List<UnitStats>();
    private int difficulty = 1;
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
            int randomHealth = Random.Range(1, localDiff + 2);
            TypesInfo.Type randomType = typesInfo.types[Random.Range(0, typesInfo.types.Length)];
            enemies.Add(new UnitStats(randomHealth, randomType));
        }
    }

    public int DifficultyScaler()
    {
        return difficulty < 5 ? difficulty++ : difficulty;
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

    public void FindTarget()
    {
        Debug.Log("наёшл цель");
    }
}
