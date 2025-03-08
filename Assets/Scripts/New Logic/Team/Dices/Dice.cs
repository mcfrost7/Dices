using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice
{
    public NewDiceConfig diceConfig; 
    private DiceSide currentSide;
    public List<ItemConfig> items;

    public DiceSide GetCurrentSide()
    {
        return currentSide;
    }
}
