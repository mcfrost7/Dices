using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewBuffConfig", menuName = "Buff/Buff Config")]
public class BuffConfig : ScriptableObject
{
    public List<ActionType> buffType = new List<ActionType>();
    public int buffPower = 0;
    public string buffName;
    public Sprite buffSprite;
}
