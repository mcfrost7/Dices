using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Configs/Item")]
[Serializable]
public class ItemConfig : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int attackBonus;
    public int defenseBonus;
    public int healthBonus;
}
