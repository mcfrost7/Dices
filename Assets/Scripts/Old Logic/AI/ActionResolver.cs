using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionResolver 
{
    // на основе подсчётов решает что выбрать

    public Unit ChooseUnitToAttack(List<Unit> unitList)
    {
        Considiration considiration = new Considiration();

        float[] score = new float[unitList.Count];
        for (int i = 0; i < score.Length; i++)
        {
            score[i] = considiration.Evaluate(unitList[i].GetComponent<Unit>().UnitStats);
        }
        return unitList[score.ToList().IndexOf(score.Max())];
    }
}
