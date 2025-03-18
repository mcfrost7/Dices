using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerUnit : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text_hp;
    [SerializeField] private TextMeshProUGUI text_moral;
    [SerializeField] private TextMeshProUGUI text_is_picked;
    [SerializeField] private Button button_dice;
    [SerializeField] private Button button_action_trigger;
    [SerializeField] private Image image_unit;

    public void UpdateText(UnitStats unitStats)
    {
        if (text_hp != null)
        {
            text_hp.text = unitStats.CurrentHealth + "/" + unitStats.Health;
        }
        if (text_moral != null)
        {
            text_moral.text = unitStats.Moral + "";
        }
    }

    public void UpdateImage(Sprite sprite)
    {
        if (image_unit != null)
        {
            image_unit.sprite = sprite;
        }
    }

    public void UpdateDice(UnitStats unitStats)
    {
        if (button_dice != null)
        {
            button_dice.GetComponent<Image>().sprite = unitStats.Type.Dice.ActionSprites[1];
            if (unitStats.IsPlayerUnit)
            {
                button_dice.onClick.AddListener(() => OnDiceClick(unitStats));
            }
        }
    }

    public void UpdateActionTrigger(Unit unit)
    {
        if (button_action_trigger != null)
        {
            button_action_trigger.onClick.AddListener(() => BattleManager.Instance.PerformAction(unit));
        }
    }

    public void UpdatePickStatus(bool isPicked)
    {
        if (text_is_picked != null)
        {
            text_is_picked.enabled = isPicked;
        }
    }

    public void OnDiceClick(UnitStats unitStats)
    {
        unitStats.IsClickable = !unitStats.IsClickable;
    }


}
