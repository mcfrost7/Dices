using System;
using System.Collections.Generic;
using UnityEngine;

public class NewUnitStats
{
    public string _name;
    public int _health;
    public int _moral;
    public int _current_exp;
    public int _level;
    public List<NewUnitStats> _upgrade_list = new List<NewUnitStats>();
    public Dice _dice;
    public List<BuffConfig> _buffs = new List<BuffConfig>();

    // Конструктор для удобства
    public NewUnitStats(string name, int health, int moral, int level, Dice dice, List<BuffConfig> buffs)
    {
        _name = name;
        _health = health;
        _moral = moral;
        _current_exp = 0;
        _level = level;
        _dice = dice;
        _buffs = buffs;
    }
}
