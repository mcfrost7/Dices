using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreator 
{
    public List<NewUnitStats> CreateEnemy(NewTileConfig config)
    {
        List<NewUnitStats> enemies = new List<NewUnitStats>();
        BattleSettings battleSettings = config.battleSettings;
        int enemyCount = GetEnemyCount(battleSettings.battleDifficulty);
        List<NewDiceConfig> possibleEnemies = GetPossibleEnemies(battleSettings.tileEnemies);
        for (int i = 0; i < enemyCount; i++)
        {
            NewUnitStats enemy = GenerateEnemy(possibleEnemies, battleSettings.battleDifficulty, i);
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
            1 => Random.Range(1, 3), // ˸���� ��� (1-2 �����)
            2 => Random.Range(2, 4), // ������� ��� (2-3 �����)
            3 => Random.Range(3, 5), // ������� ��� (3-4 �����)
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
            Debug.LogWarning("��� ��������� ������ � TileEnemies!");
            return null;
        }

        // �������� ���������� �����
        NewDiceConfig enemyDice = possibleEnemies[Random.Range(0, possibleEnemies.Count)];
        int generatedHealth = GenerateHealth(enemyDice.sides, difficulty);
        // ������ NewUnitStats �� ������ NewDiceConfig
        Dice dice = new Dice(enemyDice);
        return new NewUnitStats(id,generatedHealth, dice);
    }

    private int GenerateHealth(List<DiceSide> diceSides, int difficulty)
    {
        if (diceSides == null || diceSides.Count == 0)
        {
            Debug.LogWarning("��� ������ � ������! ������������ ������� ��������.");
            return 10 * difficulty;
        }

        int baseHealth = 0;
        foreach (var side in diceSides)
        {
            baseHealth += side.power;
        }
        baseHealth /= diceSides.Count;
        int difficultyMultiplier = difficulty switch
        {
            1 => Random.Range(1, 3),
            2 => Random.Range(2, 5),
            3 => Random.Range(3, 6),
            _ => Random.Range(1, 3)
        };

        return Mathf.RoundToInt(baseHealth + difficultyMultiplier);
    }


}
