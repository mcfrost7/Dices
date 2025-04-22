using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreator 
{
    public List<NewUnitStats> CreateEnemy(NewTileConfig config)
    {
        List<NewUnitStats> enemies = new List<NewUnitStats>();
        bool isBoss = config.tileType == TileType.BossTile;
        int difficulty = 0;
        TileEnemies tileEnemies = null;

        if (isBoss)
        {
            difficulty = config.bossSettings.battleDifficulty;
            tileEnemies = config.bossSettings.tileEnemies;
        }
        else
        {
            difficulty = config.battleSettings.battleDifficulty;
            tileEnemies = config.battleSettings.tileEnemies;
        }

        int enemyCount = GetEnemyCount(difficulty);
        List<NewDiceConfig> possibleEnemies = GetPossibleEnemies(tileEnemies);
        for (int i = 0; i < enemyCount; i++)
        {
            NewUnitStats enemy = GenerateEnemy(possibleEnemies, difficulty, i);
            if (enemy != null)
            {
                enemies.Add(enemy);
            }
        }
        return enemies;
    }


    private int GetEnemyCount(int difficulty)
    {
        return difficulty switch
        {
            1 => Random.Range(1, 3),    
            2 => Random.Range(2, 4),    
            3 => Random.Range(3, 5),    
            4 => Random.Range(4, 5),    
            5 => 5,    
            6 => 5,    
            7 => 5,    
            8 => 5,   
            9 => 5,  
            10 => 5, 
            _ => Random.Range(1, 3)     
        };
    }

    private List<NewDiceConfig> GetPossibleEnemies(TileEnemies tileEnemies)
    {
        return tileEnemies != null ? tileEnemies.newDiceConfigs : new List<NewDiceConfig>();
    }

    private NewUnitStats GenerateEnemy(List<NewDiceConfig> possibleEnemies, int difficulty, int id)
    {
        if (possibleEnemies.Count == 0)
        {
            Debug.LogWarning("Нет доступных врагов в TileEnemies!");
            return null;
        }

        // Выбираем случайного врага
        NewDiceConfig enemyDice = possibleEnemies[Random.Range(0, possibleEnemies.Count)];
        int generatedHealth = GenerateHealth(enemyDice.sides, difficulty);
        // Создаём NewUnitStats на основе NewDiceConfig
        Dice dice = new Dice(enemyDice);
        return new NewUnitStats(id,generatedHealth, dice);
    }

    private int GenerateHealth(List<DiceSide> diceSides, int difficulty)
    {
        if (diceSides == null || diceSides.Count == 0)
        {
            Debug.LogWarning("Нет граней у кубика! Используется базовое значение.");
            return 10 * difficulty;
        }

        int baseHealth = 1;
        foreach (var side in diceSides)
        {
            baseHealth += side.power;
        }
        baseHealth /= diceSides.Count;
        int difficultyMultiplier = difficulty switch
        {
            1 => Random.Range(1, 2),
            2 => Random.Range(2, 3),
            3 => Random.Range(3, 4),
            4 => Random.Range(4, 6),
            5 => Random.Range(5, 7),
            6 => Random.Range(6, 8),
            7 => Random.Range(7, 9),
            8 => Random.Range(8, 10),
            9 => Random.Range(9, 11),
            10 => Random.Range(10, 12),
            _ => Random.Range(3, 6)
        };

        return Mathf.RoundToInt(baseHealth + difficultyMultiplier);
    }


}
