using UnityEngine;

public class Unit 
{
    [SerializeField] private int health;
    [SerializeField] private int moral;
    [SerializeField] private int inventory;
    [SerializeField] private Dice dice;
    private Sprite sprite;

    public void Init(int health, int moral, int inventory, Dice dice, Sprite sprite)
    {
        this.health = health;
        this.moral = moral;
        this.inventory = inventory;
        this.dice = dice;
        this.sprite = sprite;
    }

    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    public int Moral
    {
        get { return moral; }
        set { moral = value; }
    }

    public int Inventory
    {
        get { return inventory; }
        set { inventory = value; }
    }

    public Dice Dice
    {
        get { return dice; }
        set { dice = value; }
    }

    public Sprite UnitSprite
    {
        get { return sprite; }
        set { sprite = value; }
    }
}
