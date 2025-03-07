using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DiceSide
{
    public ActionType actionType;
    public int power;
    public int duration;
    public Sprite sprite;

    public DiceSide(ActionType actionType, int power, int duration)
    {
        this.actionType = actionType;
        this.power = power;
        this.duration = duration;
        this.sprite = LoadSprite(actionType);
    }

    private Sprite LoadSprite(ActionType type)
    {
        return Resources.Load<Sprite>($"Sprites/{type.ToString()}");
    }
}
