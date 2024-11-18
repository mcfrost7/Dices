using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Considiration
{
    // Оцениваем юнита и возвращаем итоговую полезность
    public float Evaluate(UnitStats stats)
    {
        if (stats == null)
        {
            Debug.LogWarning("Передан пустой объект UnitStats.");
            return 0f;
        }

        // Вычисляем отдельные оценки
        float healthScore = EvaluateHealth(stats);
        float moralScore = EvaluateMoral(stats);
        float typeScore = EvaluateDice(stats);
        float levelScore = EvaluateLevel(stats);
        float randomScore = Random.Range(0.4f, 1f);
        // Итоговая оценка: средневзвешенное значение (можно адаптировать)
        float totalUtility = ((healthScore * 0.5f) + (moralScore * 0.3f) + (typeScore * 0.2f)) * levelScore * randomScore ;
        return Mathf.Clamp01(totalUtility); // Ограничиваем итог в пределах [0, 1]
    }

    private float EvaluateHealth(UnitStats stats)
    {
        // Чем ниже текущее здоровье, тем выше оценка (для атаки врага)
        float score = 1f - (float)stats.CurrentHealth / stats.Health;
        return score;
    }
    private float EvaluateMoral(UnitStats stats)
    {
        // Чем ниже мораль, тем выше приоритет (напр., для поддержки союзника)
        float score = (float)stats.Moral /  10;
        return score;
    }


    private float EvaluateDice(UnitStats stats)
    {
        // Определяем приоритет на основе дайса юнита
        float score = 0f;
        for (int i = 0; i < stats.Type.Dice.DiceActions.Count;i++)
        {
            if (stats.Type.Dice.DiceActions[i].ActionType  == ActionType.None)
                score += 0f;
            else if (stats.Type.Dice.DiceActions[i].ActionType == ActionType.Attack)
                score += 0.1f;
            else if (stats.Type.Dice.DiceActions[i].ActionType == ActionType.Heal)
                score += 0.03f;
            else if (stats.Type.Dice.DiceActions[i].ActionType == ActionType.LifeSteal)
                score += 0.12f;
        }
        return score;
    }

    private float EvaluateLevel(UnitStats stats)
    {
        float score = 0f;
        switch (stats.Type.Level)
        {
            case 1:
                score = 0.5f;
                break;
            case 2:
                score = 0.75f;
                break;
            case 3:
                score = 1f;
                break;
            case 0:
                score = 0f;
                break;
        }
        return score;
    }
}

