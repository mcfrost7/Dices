using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Dice
{
    [SerializeField] public NewDiceConfig _diceConfig;
    [SerializeField] private DiceSide _currentSide;
    [SerializeField] public List<ItemInstance> _items;

    public Dice(NewDiceConfig diceConfig)
    {
        this._diceConfig = diceConfig.Clone();


        // Check if diceConfig is null or sides array is empty/null
        if (diceConfig != null && diceConfig.sides != null && diceConfig.sides.Count > 0)
        {
            this._currentSide = diceConfig.sides[0];
        }
        else
        {
            // Handle the null case - either log warning or set a default
            Debug.LogWarning("Creating dice with null config or empty sides array");
            this._currentSide = null; // Make sure your code can handle null _currentSide
        }
    }
    public Dice(NewDiceConfig diceConfig, List<ItemInstance> items)
    {
        this._diceConfig = diceConfig.Clone();
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


[Serializable]
public class SerializableDice
{
    public string diceConfigName; // Store the name of the dice config
    public List<SerializableItemConfig> items = new List<SerializableItemConfig>();

    public SerializableDice(Dice dice)
    {
        if (dice == null) return;

        diceConfigName = dice._diceConfig.OriginalName;

        if (dice._items != null)
        {
            foreach (var item in dice._items)
            {
                items.Add(new SerializableItemConfig(item));
            }
        }
    }

    public Dice ToDice()
    {
        // Load the dice config by name if available
        NewDiceConfig config = !string.IsNullOrEmpty(diceConfigName) ?
            Resources.Load<NewDiceConfig>("Configs/Dices/Spacemarines/Tier 1/" + diceConfigName) : null;
        if (config == null)
            config = !string.IsNullOrEmpty(diceConfigName) ?
            Resources.Load<NewDiceConfig>("Configs/Dices/Spacemarines/Tier 2/" + diceConfigName) : null;
        if (config == null) 
            config = !string.IsNullOrEmpty(diceConfigName) ?
            Resources.Load<NewDiceConfig>("Configs/Dices/Spacemarines/Tier 3/" + diceConfigName) : null;

        Dice dice = new Dice(config);

        if (items != null)
        {
            dice._items = new List<ItemInstance>();
            foreach (var serItem in items)
            {
                dice._items.Add(serItem.ToItemInstance());
            }
        }

        return dice;
    }
}
