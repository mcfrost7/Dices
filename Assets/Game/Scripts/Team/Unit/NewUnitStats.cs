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
    public int _currentMoral;
    public int _baseMoral;
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
        _currentMoral = moral;
        _baseMoral = moral;
        _current_exp = 0;
        _current_defense = 0;
        _level = level;
        _dice = dice;
        _buffs = buffs;
    }

    public NewUnitStats(int id, int health,Dice dice)
    {
        _ID = id;
        _name = dice._diceConfig.name;
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
    public int _currentMoral;
    public int _baseMoral;
    public int _current_health;
    public int _level;
    public int _ID;
    public int _current_exp;
    public List<int> _upgrade_list = new List<int>();
    public SerializableDice _dice;
    public List<string> _buffName = new List<string>(); // Store buff IDs or names

    public SerializableUnitStats(NewUnitStats unit)
    {
        if (unit == null) return;

        _name = unit._name;
        _health = unit._health;
        _current_health = unit._current_health;
        _currentMoral = unit._currentMoral;
        _baseMoral = unit._baseMoral;
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

        if (unit._buffs != null)
        {
            foreach (var buff in unit._buffs)
            {
                _buffName.Add(buff.ConfigName);
            }
        }
    }

    public NewUnitStats ToUnitStats()
    {
        NewUnitStats unit = new NewUnitStats(
            _name,
            _health,
            _currentMoral,
            _level,
            _dice != null ? _dice.ToDice() : null,
            new List<BuffConfig>() 
        );
        unit._baseMoral  = _baseMoral;
        unit._ID = _ID;
        unit._current_exp = _current_exp;
        unit._current_health = _current_health;
        unit._upgrade_list = new List<int>(_upgrade_list);

        if (_buffName != null && _buffName.Count > 0)
        {
            unit._buffs = new List<BuffConfig>();
            foreach (var buffConfig in _buffName)
            {
                // Load buff by ID from Resources
                BuffConfig buff = Resources.Load<BuffConfig>($"Configs/Buffs/{buffConfig}");
                if (buff != null)
                {
                    unit._buffs.Add(buff);
                }
            }
        }

        return unit;
    }
}