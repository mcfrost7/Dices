using UnityEngine;

public class Unit : MonoBehaviour
{
    private int health;
    private int currentHealth;
    private int moral;
    private int inventory;
    private TypesInfo.Type type;
    private string abilities;
    private Sprite sprite;

    public void Init(int health, int moral, int inventory, TypesInfo.Type type)
    {
        this.Health = health;
        this.CurrentHealth = health;
        this.Moral = moral;
        this.Inventory = inventory;
        this.Type = type;
        this.Abilities = "none";
        this.Sprite = type.sprite;
    }

    public Sprite GetDiceSideSprite(DiceConfig.ActionType actionType)
    {
        if (type.dice != null)
        {
            return type.dice.GetSpriteForAction(actionType);
        }
        return null;
    }



    public void PerformDiceAction(DiceConfig.ActionType actionType, Unit target)
    {
        switch (actionType)
        {
            case DiceConfig.ActionType.Attack:
                type.dice.PerformAttack(target, 1); // передаём, например, силу атаки 1
                break;

            case DiceConfig.ActionType.Heal:
                type.dice.PerformHeal(this, 1); // лечим текущий юнит на 5
                break;

            case DiceConfig.ActionType.LifeSteal:
                type.dice.PerformLifeSteal(target, this, 1); // передаём 7 урона/лечения
                break;

            default:
                Debug.LogWarning("Неизвестный тип действия дайса.");
                break;
        }
    }

    public void Init(int health, TypesInfo.Type type)
    {
        this.Health = health;
        this.currentHealth = health;
        this.Type = type;
        this.Abilities = "none";
        this.Sprite = type.sprite;
    }

    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public string Abilities { get => abilities; set => abilities = value; }
    public int Health { get => health; set => health = value; }
    public int Moral { get => moral; set => moral = value; }
    public int Inventory { get => inventory; set => inventory = value; }
    public TypesInfo.Type Type { get => type; set => type = value; }
    public Sprite Sprite { get => sprite; set => sprite = value; }
}
