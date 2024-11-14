using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiceConfig", menuName = "ScriptableObjects/DiceConfig")]
public class DiceConfig : ScriptableObject
{
    [SerializeField] private Sprite[] actionSprites;
    [SerializeField] private DiceAction[] diceActions;
    [SerializeField] private ActionType current_dice_side;

    public Sprite[] ActionSprites { get => actionSprites; set => actionSprites = value; }
    public DiceAction[] DiceActions { get => diceActions; set => diceActions = value; }
    public ActionType Current_dice_side { get => current_dice_side; set => current_dice_side = value; }

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
        TakeDamage(power, target);
    }

    public void PerformHeal(Unit target, int power)
    {
        // Используем метод UnitStatus для обновления здоровья
        target.UnitStats.UpdateHealth(power);
        if (target.UnitStats.CurrentHealth > target.UnitStats.Health)
        {
            target.UnitStats.UpdateHealth(target.UnitStats.Health - target.UnitStats.CurrentHealth); // Не даем выйти за пределы максимума
        }
    }

    public void PerformLifeSteal(Unit target, Unit user, int power)
    {
        // Первая часть: атакуем цель
        TakeDamage(power, target);

        // Вторая часть: восстанавливаем здоровье пользователю
        user.UnitStats.UpdateHealth(power);
        if (user.UnitStats.CurrentHealth > user.UnitStats.Health)
        {
            user.UnitStats.UpdateHealth(user.UnitStats.Health - user.UnitStats.CurrentHealth); // Не даем выйти за пределы максимума
        }
    }

    public void TakeDamage(int damage, Unit target)
    {
        // Уменьшаем здоровье у цели
        target.UnitStats.UpdateHealth(-damage);

        if (target.UnitStats.CurrentHealth <= 0)
        {
            Die(target);
        }
    }

    private void Die(Unit target)
    {
        Debug.Log($"{target.UnitStats.Type.TypeName} погиб.");

        BattleManager.Instance.OnUnitDeath(target.UnitStats);
    }

    // Метод для получения спрайта в зависимости от типа действия
    public Sprite GetSpriteForAction(ActionType actionType)
    {
        // Индекс соответствует порядку перечисления ActionType
        int index = (int)actionType;
        if (index >= 0 && index < ActionSprites.Length)
        {
            return ActionSprites[index];
        }
        return null; // Если спрайт не найден, возвращаем null
    }
}
