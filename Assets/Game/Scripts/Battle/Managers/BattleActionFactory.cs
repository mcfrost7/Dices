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
        _actions.Add(ActionType.MoralDamage, new MoraleDamageAction());
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
        target.TakeDamage(power, source);
    }

    public bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        return BattleController.Instance.UnitsObj.Contains(source) != BattleController.Instance.UnitsObj.Contains(target);
    }
}

public class LifestealAction : IBattleAction
{
    public void Execute(BattleUnit source, BattleUnit target, int power, int duration)
    {
        int oldHealth = target.UnitData._current_health;
        target.TakeDamage(power, source);
        int damageDealt = oldHealth - target.UnitData._current_health;

        source.Heal(damageDealt);
    }

    public bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        return BattleController.Instance.UnitsObj.Contains(source) != BattleController.Instance.UnitsObj.Contains(target);
    }
}

public class HealAction : IBattleAction
{
    public void Execute(BattleUnit source = null, BattleUnit target = null, int power = 0, int duration = 0)
    {
        target.Heal(power);
    }

    public bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        return BattleController.Instance.UnitsObj.Contains(source) == BattleController.Instance.UnitsObj.Contains(target);
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
        return BattleController.Instance.UnitsObj.Contains(source) == BattleController.Instance.UnitsObj.Contains(target);
    }
}

public class ShieldBashAction : IBattleAction
{
    public void Execute(BattleUnit source, BattleUnit target, int power, int duration)
    {
        int shieldPower = source.UnitData._current_defense + power;
        if (shieldPower <= 0) return;

        target.TakeDamage(shieldPower, source);
        source.UnitData._current_defense = 0;
    }

    public bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        return BattleController.Instance.UnitsObj.Contains(source) != BattleController.Instance.UnitsObj.Contains(target);
    }
}

public class HealthAttackAction : IBattleAction
{
    public void Execute(BattleUnit source, BattleUnit target, int power, int duration)
    {
        target.TakeDamage(power, source);
        source.TakeDamage(power); 
    }

    public bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        return BattleController.Instance.UnitsObj.Contains(source) != BattleController.Instance.UnitsObj.Contains(target);
    }
}

public class MoraleAction : IBattleAction
{
    public void Execute(BattleUnit source, BattleUnit target, int power, int duration)
    {
        target.ModifyMorale(power);
    }

    public bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        return BattleController.Instance.UnitsObj.Contains(source) == BattleController.Instance.UnitsObj.Contains(target);
    }
}

public class MoraleDamageAction : IBattleAction
{
    public void Execute(BattleUnit source, BattleUnit target, int power, int duration)
    {
        source.ModifyMorale(power);
    }

    public bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        return BattleController.Instance.UnitsObj.Contains(source) == BattleController.Instance.UnitsObj.Contains(target);
    }
}

public class DefaultAction : IBattleAction
{
    public void Execute(BattleUnit source, BattleUnit target, int power, int duration)
    {
        target.TakeDamage(power, source);
    }

    public bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        return true;
    }
}
