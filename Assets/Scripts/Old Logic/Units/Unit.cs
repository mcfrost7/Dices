using UnityEngine;

public class Unit : MonoBehaviour
{
    private UnitStats unitStats;
    private bool can_act, can_be_interacted;       

    private void Awake()
    {
        UiController = gameObject.GetComponent<UIControllerUnit>();
    }
    public void Init(UnitStats unitStats)
    {
        this.UnitStats = unitStats;
        Can_act = true;
        Can_be_interacted = true;
    }

    public Sprite GetDiceSideSprite(ActionType actionType)
    {
        if (UnitStats.Type.Dice != null)
        {
            return UnitStats.Type.Dice.GetSpriteForAction(actionType);
        }
        return null;
    }
    public void UpdateUI(UnitStats unitStats, int index, Unit unit)
    {
        UiController.UpdateText(unitStats);
        UiController.UpdateImage(unitStats.Sprite);
        UiController.UpdateDice(unitStats);
        UiController.UpdateActionTrigger(unit);
    }


    public void PerformDiceAction(DiceConfig.DiceAction dice_action, Unit target)
    {
        if (dice_action == null)
        {
            Debug.LogWarning("Неизвестный тип действия дайса.");
            return;
        }

        int power = dice_action.Power;

        switch (dice_action.ActionType)
        {
            case ActionType.Attack:
                UnitStats.Type.Dice.PerformAttack(target, power);
                break;

            case ActionType.Heal:
                UnitStats.Type.Dice.PerformHeal(target, power);
                break;

            case ActionType.LifeSteal:
                UnitStats.Type.Dice.PerformLifeSteal(target, this, power);
                break;

            case ActionType.None:
                Debug.Log("Ничего не происходит.");
                break;
            default:
                Debug.LogWarning("Неизвестный тип действия дайса.");
                break;
        }
    }

    public UnitStats UnitStats { get => unitStats; set => unitStats = value; }
    public bool Can_be_interacted { get => can_be_interacted; set => can_be_interacted = value; }
    public bool Can_act { get => can_act; set => can_act = value; }
    public UIControllerUnit UiController { get; set;}
}
