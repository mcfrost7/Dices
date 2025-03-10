using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Dice
{
    [SerializeField] public NewDiceConfig diceConfig;
    [SerializeField] private DiceSide currentSide;
    [SerializeField] public List<ItemConfig> items;

    public DiceSide GetCurrentSide()
    {
        return currentSide;
    }
}
