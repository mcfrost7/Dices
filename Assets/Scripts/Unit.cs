using System.Runtime.InteropServices;
using UnityEngine;

public class Unit 
{
    private int health;
    private int currentHealth;
    private int moral;
    private int inventory;
    private Sprite[] dice;
    private Sprite sprite;
    private string type;
    private string abilities;

    public void Init(int health, int moral, Sprite[] dice, int inventory, string type, Sprite sprite)
    {
        this.Health = health;
        this.CurrentHealth = health;
        this.Moral = moral;
        this.Inventory = inventory;
        this.Type = type;
        this.Sprite = sprite;
        this.Abilities = "none";
        this.DiceSprite = dice;
    }

    public void Init (int health, string type, Sprite sprite, Sprite[] diceSprite)
    {
        this .Health = health;
        this.currentHealth = health;  
        this .DiceSprite = diceSprite;  
        this.Type = type;  
        this.Sprite = sprite;
        this.Abilities = "none";
    }   



    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public string Abilities { get => abilities; set => abilities = value; }
    public int Health { get => health; set => health = value; }
    public int Moral { get => moral; set => moral = value; }
    public int Inventory { get => inventory; set => inventory = value; }

    public Sprite Sprite { get => sprite; set => sprite = value; }
    public string Type { get => type; set => type = value; }
    public Sprite[] DiceSprite { get => dice; set => dice = value; }
}
