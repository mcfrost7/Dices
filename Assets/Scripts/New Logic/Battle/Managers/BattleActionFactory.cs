// �������� ��������� ��� ��������
using System.Collections.Generic;
using UnityEngine;



// �������� ������� ��������
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
        _actions.Add(ActionType.None, new DefaultAction());
    }

    public IBattleAction GetAction(ActionType type)
    {
        if (_actions.TryGetValue(type, out IBattleAction action))
            return action;
        return _actions[ActionType.None]; // �������� �� ���������
    }
}

// ���������� ��������
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
        // �������� ���������� ���� ��� �����
        bool isSourcePlayer = BattleController.Instance.UnitsObj.Contains(source);
        bool isTargetPlayer = BattleController.Instance.UnitsObj.Contains(target);
        return isSourcePlayer != isTargetPlayer; // ����� ��������� ������ ������
    }
}
public class LifestealAction : IBattleAction
{
    public void Execute(BattleUnit source, BattleUnit target, int power, int duration)
    {
        int damageDealt;

        if (target.UnitData._current_defense > 0)
        {
            // ��������� ������
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
            // ��� ������ ���� ���� ��������
            damageDealt = Mathf.Min(power, target.UnitData._current_health);
            target.UnitData._current_health = Mathf.Max(0, target.UnitData._current_health - power);
        }

        // ��������������� �������� ��������������� ����������� �����
        source.UnitData._current_health = Mathf.Min(
            source.UnitData._current_health + damageDealt,
            source.UnitData._health
        );
    }

    public bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        // �������� ���������� ���� ��� �����
        bool isSourcePlayer = BattleController.Instance.UnitsObj.Contains(source);
        bool isTargetPlayer = BattleController.Instance.UnitsObj.Contains(target);
        return isSourcePlayer != isTargetPlayer; // ����� ��������� ������ ������
    }
}

public class HealAction : IBattleAction
{
    public void Execute(BattleUnit source, BattleUnit target, int power, int duration)
    {
        target.UnitData._current_health = Mathf.Min(
            target.UnitData._current_health + power,
            target.UnitData._health
        );
    }

    public bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        // �������� ���������� ���� ��� �������
        bool isSourcePlayer = BattleController.Instance.UnitsObj.Contains(source);
        bool isTargetPlayer = BattleController.Instance.UnitsObj.Contains(target);
        return isSourcePlayer == isTargetPlayer; // ����� ������ ������ ���������
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
        // �������� ���������� ���� ��� �������
        bool isSourcePlayer = BattleController.Instance.UnitsObj.Contains(source);
        bool isTargetPlayer = BattleController.Instance.UnitsObj.Contains(target);
        return isSourcePlayer == isTargetPlayer; // ����� ������ ������ ���������
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
        return true; // ����� ���� �������
    }
}