using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NewUnitStats
{
    public string _name;
    public int _health;
    public int _moral;
    public int _current_exp;
    public int _level;
    public List<NewUnitStats> _upgrade_list = new List<NewUnitStats>();
    public NewDiceConfig _dice;
    public BuffConfig _buff;

    // Конструктор для удобства
    public NewUnitStats(string name, int health, int moral, int level, NewDiceConfig dice, BuffConfig buff)
    {
        _name = name;
        _health = health;
        _moral = moral;
        _current_exp = 0;
        _level = level;
        _dice = dice;
        _buff = buff;
    }
}
