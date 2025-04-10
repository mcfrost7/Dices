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
    }

}
