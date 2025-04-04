using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Dice
{
    [SerializeField] public NewDiceConfig _diceConfig;
    [SerializeField] private DiceSide _currentSide;
    [SerializeField] public List<ItemConfig> _items;

    public Dice(NewDiceConfig diceConfig)
    {
        this._diceConfig = diceConfig;
        this._currentSide = diceConfig.sides[0];
    }
    public Dice(NewDiceConfig diceConfig, List<ItemConfig> items)
    {
        this._diceConfig = diceConfig;
        this._currentSide = diceConfig.sides[0];
        this._items = items;
    }

    public DiceSide GetCurrentSide()
    {
        return _currentSide;
    }

    public void SetCurrentSide(DiceSide side)
    {
        _currentSide = side;
    }
}
