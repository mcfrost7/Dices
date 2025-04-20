using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DiceSide
{
    public ActionType actionType;
    public int power;
    public int bonus = 0;
    public int sideIndex;
    public int duration;
    public Sprite sprite;
    public ActionSide ActionSide;

}


public enum ActionSide
{
    None,
    Enemy,
    Ally
}
