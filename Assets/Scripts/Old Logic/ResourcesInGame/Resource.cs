using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewResource", menuName = "Game/Resource")]
public class Resource : ScriptableObject
{
    [SerializeField] private string resourceName; // Название ресурса
    [SerializeField] private int amount;         // Количество ресурса
    [SerializeField] private Sprite icon;        // Иконка ресурса
    [SerializeField] private ResourcesType resourcesType;
    // Свойства для доступа к полям
    public string Name => resourceName;
    public int Amount => amount;
    public Sprite Icon => icon;
    public ResourcesType ResourcesType { get => resourcesType; set => resourcesType = value; }

    // Метод для добавления ресурса
    public void AddAmount(int value)
    {
        amount += value;
    }

    // Метод для уменьшения ресурса
    public void SubtractAmount(int value)
    {
        amount = Mathf.Max(amount - value, 0); // Не позволяет уйти в отрицательные значения
    }

    // Метод для обновления количества ресурса
    public void SetAmount(int value)
    {
        amount = Mathf.Max(value, 0); // Устанавливает новое количество
    }

    // Метод для обновления иконки
    public void SetIcon(Sprite newIcon)
    {
        icon = newIcon;
    }
    public void Initialize(string name, int initialAmount, Sprite resourceIcon, ResourcesType type)
    {
        resourceName = name;
        amount = initialAmount;
        icon = resourceIcon;
        resourcesType = type;
    }

}

