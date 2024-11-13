using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private Unit[] enemies;
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

        // Очищаем массив врагов перед созданием новых
        enemies = new Unit[localDiff];

        for (int i = 0; i < localDiff; i++)
        {
            enemies[i] = new Unit();
            int randomHealth = Random.Range(1, localDiff + 2);
            TypesInfo.Type randomType = typesInfo.types[Random.Range(0, typesInfo.types.Length)];
            enemies[i].Init(randomHealth, randomType);
        }
    }

    public void RemoveEnemy(int indexToRemove)
    {
        // Преобразуем массив в List для удобства работы
        List<Unit> enemyList = new List<Unit>(enemies);

        // Удаляем элемент по индексу
        if (indexToRemove >= 0 && indexToRemove < enemyList.Count)
        {
            enemyList.RemoveAt(indexToRemove);
        }

        // Преобразуем List обратно в массив
        enemies = enemyList.ToArray();
    }


    public int DifficultyScaler()
    {
        return difficulty < 5 ? difficulty++ : difficulty;
    }

    public Unit[] Enemies
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
