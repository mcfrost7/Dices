using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDiceConfig", menuName = "Dice/Dice Config")]
public class NewDiceConfig : ScriptableObject
{
    public List<DiceSide> sides = new List<DiceSide>();
    public Sprite _unitSprite;
}
