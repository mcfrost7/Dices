using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NewUnitStats
{
    public int _ID;
    public string _name;
    public int _health;
    public int _current_health;
    public int _moral;
    public int _current_defense;
    public int _current_exp;
    public int _level;
    public List<int> _upgrade_list = new List<int>();
    public Dice _dice;
    public List<BuffConfig> _buffs = new List<BuffConfig>();

    // Конструктор для удобства
    public NewUnitStats(string name, int health, int moral, int level, Dice dice, List<BuffConfig> buffs)
    {
        _name = name;
        _health = health;
        _current_health = health;
        _moral = moral;
        _current_exp = 0;
        _current_defense = 0;
        _level = level;
        _dice = dice;
        _buffs = buffs;
    }

    public NewUnitStats(int id, int health,Dice dice)
    {
        _ID = id;
        _health = health;
        _current_health = health;
        _current_defense=0;
        _dice=dice;
        _buffs = null;
    }

}
[Serializable]
public class SerializableUnitStats
{
    public string _name;
    public int _health;
    public int _moral;
    public int _current_health;
    public int _level;
    public int _ID;
    public int _current_exp;
    public List<int> _upgrade_list = new List<int>();
    public SerializableDice _dice;
    public List<int> _buffIds = new List<int>(); // Store buff IDs or names

    public SerializableUnitStats(NewUnitStats unit)
    {
        if (unit == null) return;

        _name = unit._name;
        _health = unit._health;
        _current_health = unit._current_health;
        _moral = unit._moral;
        _level = unit._level;
        _ID = unit._ID;
        _current_exp = unit._current_exp;

        if (unit._upgrade_list != null)
        {
            _upgrade_list = new List<int>(unit._upgrade_list);
        }

        if (unit._dice != null)
        {
            _dice = new SerializableDice(unit._dice);
        }

        // Store buff IDs
        if (unit._buffs != null)
        {
            foreach (var buff in unit._buffs)
            {
                // Assuming each buff has an ID or a way to identify it
                _buffIds.Add(buff.buffId);
            }
        }
    }

    public NewUnitStats ToUnitStats()
    {
        NewUnitStats unit = new NewUnitStats(
            _name,
            _health,
            _moral,
            _level,
            _dice != null ? _dice.ToDice() : null,
            new List<BuffConfig>() // We'll fill this later
        );

        unit._ID = _ID;
        unit._current_exp = _current_exp;
        unit._current_health = _current_health;
        unit._upgrade_list = new List<int>(_upgrade_list);

        // Load buffs from IDs
        if (_buffIds != null && _buffIds.Count > 0)
        {
            unit._buffs = new List<BuffConfig>();
            foreach (var buffId in _buffIds)
            {
                // Load buff by ID from Resources
                BuffConfig buff = Resources.Load<BuffConfig>($"Buffs/Buff_{buffId}");
                if (buff != null)
                {
                    unit._buffs.Add(buff);
                }
            }
        }

        return unit;
    }
}