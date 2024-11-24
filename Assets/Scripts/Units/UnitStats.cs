using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static DiceConfig;
using Random = UnityEngine.Random;

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
    public List<Names> AvailableNames { get; set; }

    public void InitializeNames()
    {
        // Инициализация списка уникальных имен
        AvailableNames = new List<Names>((Names[])Enum.GetValues(typeof(Names)));
        ShuffleList(AvailableNames);
    }

    public string GetUniqueName()
    {
        // Проверяем, что список инициализирован
        if (AvailableNames == null || AvailableNames.Count == 0)
        {
            InitializeNames();
        }

        // Получаем имя и удаляем его из списка
        Names uniqueName = AvailableNames[0];
        AvailableNames.RemoveAt(0);

        return uniqueName.ToString();
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

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
        Name = GetUniqueName();
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

    public void AddExperience(int amount)
    {
        Experience += amount;
        // Можно добавить логику для проверки достижения нового уровня, например
        if (Experience >= 10)
        {
            LevelUp();
        }
    }

    // Пример метода повышения уровня
    private void LevelUp()
    {
        // Логика повышения уровня, например:
        Debug.Log($"{Name} достиг нового уровня!");
    }

}
