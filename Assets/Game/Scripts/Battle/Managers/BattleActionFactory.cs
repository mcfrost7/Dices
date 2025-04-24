// Создайте интерфейс для действий
using System.Collections.Generic;
using UnityEngine;



// Создайте фабрику действий
public class BattleActionFactory : MonoBehaviour
{
    public static BattleActionFactory Instance { get; private set; }

    private Dictionary<ActionType, IBattleAction> _actions = new Dictionary<ActionType, IBattleAction>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        RegisterActions();
    }

    private void RegisterActions()
    {
        _actions.Add(ActionType.Attack, new AttackAction());
        _actions.Add(ActionType.LifeSteal, new LifestealAction());
        _actions.Add(ActionType.Heal, new HealAction());
        _actions.Add(ActionType.Defense, new DefenseAction());
        _actions.Add(ActionType.ShieldBash, new ShieldBashAction());
        _actions.Add(ActionType.None, new DefaultAction());
        _actions.Add(ActionType.Moral, new MoraleAction());
        _actions.Add(ActionType.HealthAttack, new HealthAttackAction());
    }

    public IBattleAction GetAction(ActionType type)
    {
        if (_actions.TryGetValue(type, out IBattleAction action))
            return action;
        return _actions[ActionType.None]; // Действие по умолчанию
    }
}

// Реализации действий
public class AttackAction : IBattleAction
{
    public void Execute(BattleUnit source, BattleUnit target, int power, int duration)
    {
        if (target.UnitData._current_defense > 0)
        {
            int remainingDamage = Mathf.Max(0, power - target.UnitData._current_defense);
            target.UnitData._current_defense = Mathf.Max(0, target.UnitData._current_defense - power);
            if (remainingDamage > 0)
            {
                target.UnitData._current_health = Mathf.Max(0, target.UnitData._current_health - remainingDamage);
            }
        }
        else
        {
            target.UnitData._current_health = Mathf.Max(0, target.UnitData._current_health - power);
        }
    }

    public bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        // Проверка валидности цели для атаки
        bool isSourcePlayer = BattleController.Instance.UnitsObj.Contains(source);
        bool isTargetPlayer = BattleController.Instance.UnitsObj.Contains(target);
        return isSourcePlayer != isTargetPlayer; // Можно атаковать только врагов
    }
}
public class LifestealAction : IBattleAction
{
    public void Execute(BattleUnit source, BattleUnit target, int power, int duration)
    {
        int damageDealt;

        if (target.UnitData._current_defense > 0)
        {
            // Пробиваем защиту
            int remainingDamage = Mathf.Max(0, power - target.UnitData._current_defense);
            target.UnitData._current_defense = Mathf.Max(0, target.UnitData._current_defense - power);
            damageDealt = remainingDamage;

            if (remainingDamage > 0)
            {
                target.UnitData._current_health = Mathf.Max(0, target.UnitData._current_health - remainingDamage);
            }
        }
        else
        {
            // Без защиты весь урон проходит
            damageDealt = Mathf.Min(power, target.UnitData._current_health);
            target.UnitData._current_health = Mathf.Max(0, target.UnitData._current_health - power);
        }

        // Восстанавливаем здоровье пропорционально нанесенному урону
        source.UnitData._current_health = Mathf.Min(
            source.UnitData._current_health + damageDealt,
            source.UnitData._health
        );
    }

    public bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        // Проверка валидности цели для атаки
        bool isSourcePlayer = BattleController.Instance.UnitsObj.Contains(source);
        bool isTargetPlayer = BattleController.Instance.UnitsObj.Contains(target);
        return isSourcePlayer != isTargetPlayer; // Можно атаковать только врагов
    }
}

public class HealAction : IBattleAction
{
    public void Execute(BattleUnit source, BattleUnit target, int power, int duration)
    {
        Heal(target.UnitData,power);
    }

    public void Heal(NewUnitStats target, int power)
    {
        target._current_health = Mathf.Min(
            target._current_health + power,
            target._health);
    }

    public bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        // Проверка валидности цели для лечения
        bool isSourcePlayer = BattleController.Instance.UnitsObj.Contains(source);
        bool isTargetPlayer = BattleController.Instance.UnitsObj.Contains(target);
        return isSourcePlayer == isTargetPlayer; // Можно лечить только союзников
    }
}

public class DefenseAction : IBattleAction
{
    public void Execute(BattleUnit source, BattleUnit target, int power, int duration)
    {
        target.UnitData._current_defense += power;
    }

    public bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        // Проверка валидности цели для лечения
        bool isSourcePlayer = BattleController.Instance.UnitsObj.Contains(source);
        bool isTargetPlayer = BattleController.Instance.UnitsObj.Contains(target);
        return isSourcePlayer == isTargetPlayer; // Можно лечить только союзников
    }
}


public class ShieldBashAction : IBattleAction
{
    public void Execute(BattleUnit source, BattleUnit target, int power, int duration)
    {
        int shieldPower = source.UnitData._current_defense + power;
        if (shieldPower <= 0) return;

        if (target.UnitData._current_defense > 0)
        {
            int remainingDamage = Mathf.Max(0, shieldPower - target.UnitData._current_defense);
            target.UnitData._current_defense = Mathf.Max(0, target.UnitData._current_defense - shieldPower);
            if (remainingDamage > 0)
            {
                target.UnitData._current_health = Mathf.Max(0, target.UnitData._current_health - remainingDamage);
            }
        }
        else
        {
            target.UnitData._current_health = Mathf.Max(0, target.UnitData._current_health - shieldPower);
        }

        // Сбросить защиту у атакующего
        source.UnitData._current_defense = 0;
    }

    public bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        bool isSourcePlayer = BattleController.Instance.UnitsObj.Contains(source);
        bool isTargetPlayer = BattleController.Instance.UnitsObj.Contains(target);
        return isSourcePlayer != isTargetPlayer; // Только враги
    }
}


public class HealthAttackAction : IBattleAction
{
    public void Execute(BattleUnit source, BattleUnit target, int power, int duration)
    {
        ApplyDamage(source, target, power);
        ApplyDamage(source, source, power);
    }

    public bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        return BattleController.Instance.UnitsObj.Contains(source) != BattleController.Instance.UnitsObj.Contains(target);
    }

    private void ApplyDamage(BattleUnit attacker, BattleUnit defender, int damage)
    {
        var unitData = defender.UnitData;

        if (unitData._current_defense > 0)
        {
            int defenseReduction = Mathf.Min(unitData._current_defense, damage);
            unitData._current_defense -= defenseReduction;
            damage -= defenseReduction;
        }

        // После обработки защиты наносим урон в здоровье
        if (damage > 0)
        {
            unitData._current_health = Mathf.Max(0, unitData._current_health - damage);
        }
    }
}


public class MoraleAction : IBattleAction
{
    public void Execute(BattleUnit source, BattleUnit target, int power, int duration)
    {

        target.UnitData._currentMoral = target.UnitData._currentMoral + power;
    }

    public bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        bool isSourcePlayer = BattleController.Instance.UnitsObj.Contains(source);
        bool isTargetPlayer = BattleController.Instance.UnitsObj.Contains(target);
        return isSourcePlayer == isTargetPlayer;
    }
}



public class DefaultAction : IBattleAction
{
    public void Execute(BattleUnit source, BattleUnit target, int power, int duration)
    {
        target.UnitData._current_health -= power;
    }

    public bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        return true; // Любая цель валидна
    }
}