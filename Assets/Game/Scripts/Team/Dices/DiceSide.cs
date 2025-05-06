using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DiceSide
{
    public ActionType actionType;
    public int power;
    public int bonus = 0;
    [HideInInspector] public int sideIndex;
    [HideInInspector] public int duration;
    public Sprite sprite;
    public ActionSide ActionSide;

    public DiceSide Clone()
    {
        return new DiceSide
        {
            actionType = this.actionType,
            sideIndex = this.sideIndex,
            power = this.power,
            duration = this.duration,
            bonus = this.bonus,
            sprite = this.sprite,
            ActionSide = this.ActionSide
        };
    }

}


public enum ActionSide
{
    None,
    Enemy,
    Ally
}
