using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour 
{
    private UnitStats unitStats; // Экземпляр UnitStatus для управления состоянием
    private bool can_act,can_be_interacted;


    [SerializeField] private TextMeshProUGUI text_hp;
    [SerializeField] private TextMeshProUGUI text_moral;
    [SerializeField] private TextMeshProUGUI text_is_picked;
    [SerializeField] private Button button_dice;
    [SerializeField] private Button button_action_trigger;
    [SerializeField] private Image image_unit;
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

    public void PerformDiceAction(DiceConfig.DiceAction dice_action, Unit target)
    {

        if (dice_action == null)
        {
            Debug.LogWarning("Неизвестный тип действия дайса.");
            return;
        }

        int power = dice_action.Power; // Извлекаем значение power из найденного действия

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
    public void UpdateUI()
    {
        if (Text_hp != null)
        {
            Text_hp.text = UnitStats.CurrentHealth + "/" + UnitStats.Health;
        }
        if (Text_moral != null)
        {
            Text_moral.text = UnitStats.Moral + "";
        }
    }

    public void UpdateImage()
    {
        Image_unit.sprite = UnitStats.Type.Sprite_type;
    }

    public void UpdateDice(bool isPlayer, int index)
    {
        Button_dice.GetComponent<Image>().sprite = UnitStats.Type.Dice.ActionSprites[1];
        if (isPlayer)
        {
            Button_dice.GetComponent<Button>().onClick.AddListener(() => BattleManager.Instance.OnDiceClick(index));
        }
    }

    public void UpdateActionTrigger(Unit unit)
    {
        Button_action_trigger.GetComponent<Button>().onClick.AddListener(() => BattleManager.Instance.PerformAction(unit));
    }
    public UnitStats UnitStats { get => unitStats; set => unitStats = value; }
    public TextMeshProUGUI Text_hp { get => text_hp; set => text_hp = value; }
    public TextMeshProUGUI Text_moral { get => text_moral; set => text_moral = value; }
    public TextMeshProUGUI Text_is_picked { get => text_is_picked; set => text_is_picked = value; }
    public Button Button_dice { get => button_dice; set => button_dice = value; }
    public Button Button_action_trigger { get => button_action_trigger; set => button_action_trigger = value; }
    public Image Image_unit { get => image_unit; set => image_unit = value; }
    public bool Can_be_interacted { get => can_be_interacted; set => can_be_interacted = value; }
    public bool Can_act { get => can_act; set => can_act = value; }
}
