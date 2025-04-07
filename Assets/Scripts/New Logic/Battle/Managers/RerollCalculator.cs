using System.Collections.Generic;
using UnityEngine;

public static class RerollCalculator
{
    public static int CalculateRerolls(List<NewUnitStats> units)
    {
        if (units == null || units.Count == 0)
            return 0;

        float total = 0;

        foreach (var unit in units)
            total += GetModifiedMorale(unit);

        float average = total / units.Count;

        return Mathf.RoundToInt(average);
    }


    private static int GetModifiedMorale(NewUnitStats unit)
    {
        int morale = unit._moral;

        foreach (var buff in unit._buffs)
        {
            if (buff.buffType == ActionType.Moral)
            {
                morale += buff.buffPower;
            }
        } 

        foreach (var item in unit._dice._items)
        {
            if (item.actionType == ActionType.Moral)
            {
                morale += item.power;
            }
        }
          
        return morale;
    }
}
