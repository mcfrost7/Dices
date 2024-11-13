using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiceConfig", menuName = "ScriptableObjects/DiceConfig")]
public class DiceConfig : ScriptableObject
{
    public Sprite[] actionSprites; // Спрайты для каждого действия
    public DiceAction[] diceActions;

    public enum ActionType
    {
        None,
        Attack,
        Defense,
        Heal,
        LifeSteal
    }

    [System.Serializable]
    public class DiceAction
    {
        public ActionType actionType;
        public int power; // сила действия, влияющая на урон или лечение
    }

    public void PerformAttack(Unit target, int power)
    {
        target.CurrentHealth -= power;
    }

    public void PerformHeal(Unit target, int power)
    {
        target.CurrentHealth += power;
        if (target.CurrentHealth > target.Health)
        {
            target.CurrentHealth = target.Health;
        }
    }

    public void PerformLifeSteal(Unit target, Unit user, int power)
    {
        target.CurrentHealth -= power;
        user.CurrentHealth += power;
        if (user.CurrentHealth > user.Health)
        {
            user.CurrentHealth = user.Health;
        }
    }

    // Метод для получения спрайта в зависимости от типа действия
    public Sprite GetSpriteForAction(ActionType actionType)
    {
        // Индекс соответствует порядку перечисления ActionType
        int index = (int)actionType;
        if (index >= 0 && index < actionSprites.Length)
        {
            return actionSprites[index];
        }
        return null; // Если спрайт не найден, возвращаем null
    }
}
