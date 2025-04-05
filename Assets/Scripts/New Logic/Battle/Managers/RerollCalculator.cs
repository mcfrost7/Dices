using System.Collections.Generic;

public static class RerollCalculator
{
    public static int CalculateRerolls(List<NewUnitStats> units)
    {
        int total = 0;

        foreach (var unit in units)
            total += GetModifiedMorale(unit);

        return total / units.Count;
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
