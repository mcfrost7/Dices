using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(fileName = "NewDiceConfig", menuName = "Dice/Dice Config")]
public class NewDiceConfig : ScriptableObject
{
    public List<DiceSide> sides = new List<DiceSide>();
    public Sprite _unitSprite;

    public void AddNewSide(DiceSide newSide)
    {
        newSide.sideIndex = sides.Count;
        sides.Add(newSide);
    }

    public void RefreshSideIndices()
    {
        for (int i = 0; i < sides.Count; i++)
        {
            sides[i].sideIndex = i;
            sides[i].bonus = 0;
        }
    }
    public void LoadImages(bool isEnemy)
    {
        string curentpath;
        string pathSM = "Sprites/Dices/TC_Dice_sides";
        string pathEnemy = "Sprites/Dices/Orc_Dice_sides";
        if (isEnemy) 
        {
            curentpath = pathEnemy;
        }else
        {
            curentpath = pathSM;
        }
        for (int i = 0; i < sides.Count; i++)
        {
            sides[i].sprite = GetSpriteForActionType(sides[i].actionType,curentpath);
            sides[i].ActionSide = GetActionSideForActionType(sides[i].actionType);
            if (sides[i].actionType == ActionType.None)
            {
                sides[i].power = 0;
            }
        }
    }

    private Sprite GetSpriteForActionType(ActionType actionType,string path)
    {
        return Resources.Load<Sprite>($"{path}/{actionType}");
    }

    private ActionSide GetActionSideForActionType(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.Attack:
            case ActionType.LifeSteal:
            case ActionType.HealthAttack:
            case ActionType.ShieldBash:
                return ActionSide.Enemy;

            case ActionType.Defense:
            case ActionType.Heal:
            case ActionType.Moral:
                return ActionSide.Ally;

            case ActionType.None:
            default:
                return ActionSide.None;
        }
    }

    public NewDiceConfig Clone()
    {
        var clone = Instantiate(this); // создаём копию ScriptableObject
        clone.sides = new List<DiceSide>();
        foreach (var side in this.sides)
        {
            clone.sides.Add(side.Clone());
        }
        return clone;
    }

}