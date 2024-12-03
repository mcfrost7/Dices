using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiceConfig", menuName = "ScriptableObjects/DiceConfig")]
public class DiceConfig : ScriptableObject
{
    [SerializeField] private Sprite[] actionSprites;
    [SerializeField] private List<DiceAction> diceActions = new List<DiceAction>();
    private DiceAction current_dice_side;

    public Sprite[] ActionSprites { get => actionSprites; set => actionSprites = value; }
    public List<DiceAction> DiceActions { get => diceActions; set => diceActions = value; }
    public DiceAction Current_dice_side { get => current_dice_side; set => current_dice_side = value; }

    [System.Serializable]
    public class DiceAction 
    {
        [SerializeField] private ActionType actionType;
        [SerializeField] private int power; // сила действия, влияющая на урон или лечение

        public ActionType ActionType { get => actionType; set => actionType = value; }
        public int Power { get => power; set => power = value; }
    }

    public void PerformAttack(Unit target, int power)
    {
        TakeDamage(power, target);
        Sounds.Instance.PlaySound(Sounds.Instance.Sounds1[0], volume: 0.7f);
    }

    public void PerformHeal(Unit target, int power)
    {
        // Используем метод UnitStatus для обновления здоровья
        target.UnitStats.UpdateHealth(power);
        Sounds.Instance.PlaySound(Sounds.Instance.Sounds1[1], volume: 0.7f);
    }

    public void PerformLifeSteal(Unit target, Unit user, int power)
    {
        user.UnitStats.UpdateHealth(power);
        Sounds.Instance.PlaySound(Sounds.Instance.Sounds1[1], volume: 0.7f);
        TakeDamage(power, target);
        Sounds.Instance.PlaySound(Sounds.Instance.Sounds1[0], volume:0.7f);
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

        BattleManager.Instance.OnUnitDeath(target);
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
