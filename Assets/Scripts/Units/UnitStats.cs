using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static DiceConfig;

public class UnitStats
{
    public GameObject unitObject;


    public int Health { get; private set; }
    public int Moral { get; private set; }
    public int CurrentHealth { get; private set; }
    //public int Inventory { get; private set; }
    //public string Abilities { get;private set; }
    public Sprite Sprite { get; private set; }
    public bool IsPlayerUnit { get; private set; }
    public TypesInfo.Type Type { get; private set; }

    public string Name { get; private set; }
    public DiceAction Current_dice_side { get; set; }

    public int Experience {  get; set; }

    public Unit Target {  get;  set; }
    
    public bool IsClickable { get; set; }

    public UnitStats()
    {

    }
    public UnitStats(int health, int moral, TypesInfo.Type type)
    {
        Health = health;
        CurrentHealth = health;
        Moral = moral;
        Sprite = type.Sprite_type;
        Type = type;
        IsPlayerUnit = true;
        Name = ((Names)Random.Range(0, System.Enum.GetValues(typeof(Names)).Length)).ToString();
        Experience = 0;
        IsClickable = true;

    }
    public UnitStats(int health, TypesInfo.Type type)
    {
        Health = health;
        CurrentHealth = health;
        Sprite = type.Sprite_type;
        Type = type;
        IsPlayerUnit = false;
        Name = ((OrcsNames)Random.Range(0, System.Enum.GetValues(typeof(OrcsNames)).Length)).ToString();
        IsClickable = true;
    }

    // Метод для обновления здоровья
    public void UpdateHealth(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth < 0) CurrentHealth = 0;  // Здоровье не может быть меньше нуля
        if (CurrentHealth > Health) CurrentHealth = Health;  // Здоровье не может быть больше максимума
    }

    // Метод для обновления морали
    public void UpdateMoral(int amount)
    {
        Moral += amount;
        if (Moral < 0) Moral = 0;  // Мораль не может быть меньше нуля
        if (Moral > 100) Moral = 100;  // Мораль не может быть больше 100
    }
}
