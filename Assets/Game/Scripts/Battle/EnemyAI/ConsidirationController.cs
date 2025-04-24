using System.Collections.Generic;
using System.Linq;

public class ConsidirationController
{
    public Dictionary<BattleUnit, float> EvaluatePlayerUnits(List<BattleUnit> playerUnits)
    {
        Dictionary<BattleUnit, float> unitScores = new Dictionary<BattleUnit, float>();

        foreach (var unit in playerUnits)
        {
            float score = CalculateUnitValue(unit.UnitData);
            unitScores[unit] = score;
        }

        return unitScores;
    }

    private float CalculateUnitValue(NewUnitStats unit)
    {
        float healthFactor = 1f - (unit._current_health / (float)unit._health); // Чем меньше ХП, тем выше приоритет
        float levelFactor = unit._level * 2f; // Чем выше уровень, тем выше приоритет
        float moralFactor = unit._currentMoral * 1.5f; // Высокая мораль → опасный юнит

        float dicePowerFactor = 0f;
        float attackFactor = 0f;
        float defensePenalty = 0f;

        foreach (var side in unit._dice._diceConfig.sides)
        {
            dicePowerFactor += side.power;

            if (side.actionType == ActionType.Attack || side.actionType == ActionType.LifeSteal)
            {
                attackFactor += side.power * 2; // Угроза от атакующих граней
            }
            else if (side.actionType == ActionType.Defense || side.actionType == ActionType.Heal)
            {
                defensePenalty += side.power * 1.5f; // Снижаем приоритет цели
            }
        }

        float buffBonus = 0f;
        float buffPenalty = 0f;

        foreach (var buff in unit._buffs)
        {
            if (buff.buffType == ActionType.Attack || buff.buffType == ActionType.LifeSteal)
            {
                buffBonus += buff.buffPower * 2; // Атакующие баффы увеличивают приоритет
            }
            else if (buff.buffType ==  ActionType.Defense || buff.buffType == ActionType.Heal)
            {
                buffPenalty += buff.buffPower * 3; // Защитные баффы снижают приоритет
            }
        }

        float baseScore = healthFactor * 10 + levelFactor + moralFactor + dicePowerFactor + attackFactor + defensePenalty - buffBonus + buffPenalty;
        float randomFactor = UnityEngine.Random.Range(0.8f, 1.2f);

        return baseScore * randomFactor;
    }


}
