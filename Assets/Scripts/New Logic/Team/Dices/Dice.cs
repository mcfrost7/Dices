using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice
{
    public DiceConfig diceConfig; 
    private DiceSide currentSide;
    private List<ItemConfig> items;

    public DiceSide GetCurrentSide()
    {
        return currentSide;
    }
}
