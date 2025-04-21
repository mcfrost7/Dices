using System.Collections.Generic;
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
        }
    }
    public void LoadImages()
    {
        for (int i = 0; i < sides.Count; i++)
        {
            sides[i].sprite = GetSpriteForActionType(sides[i].actionType);
            sides[i].ActionSide = GetActionSideForActionType(sides[i].actionType);
            if (sides[i].actionType == ActionType.None)
            {
                sides[i].power = 0;
            }
        }
    }

    private Sprite GetSpriteForActionType(ActionType actionType)
    {
        return Resources.Load<Sprite>($"Sprites/Dices/TC_Dice_sides/{actionType}");
    }

    private ActionSide GetActionSideForActionType(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.Attack:
            case ActionType.LifeSteal:
                return ActionSide.Enemy;

            case ActionType.Defense:
            case ActionType.Heal:
            case ActionType.Moral:
            case ActionType.HP:
                return ActionSide.Ally;

            case ActionType.None:
            default:
                return ActionSide.None;
        }
    }
}