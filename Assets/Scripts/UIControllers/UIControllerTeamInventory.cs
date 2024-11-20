using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerTeamInventory : MonoBehaviour
{
    [SerializeField] private GameObject[] sprite_sides;
    [SerializeField] private TextMeshProUGUI text_name;

    public void DrawAllPanels(UnitStats stats)
    {
        DrawDiceSides(stats);
        DrawUnitName(stats);
    }

    private void DrawDiceSides(UnitStats stats)
    {
        int i = 0;
        foreach (var item in stats.Type.Dice.ActionSprites)
        {
            sprite_sides[i].GetComponent<Image>().sprite = item;
            i++;
        }
    }

    private void DrawUnitName(UnitStats stats) 
    {
        var (firstName, secondName) = SplitName(stats.Name);
        text_name.text = firstName + " " + secondName;
    }

    public static (string firstName, string lastName) SplitName(string fullName)
    {
        if (string.IsNullOrEmpty(fullName))
        {
            throw new ArgumentException("Имя не может быть пустым или null.");
        }

        // Ищем, где начинается фамилия (первая заглавная буква после первой буквы)
        int splitIndex = -1;
        for (int i = 1; i < fullName.Length; i++)
        {
            if (char.IsUpper(fullName[i]))
            {
                splitIndex = i;
                break;
            }
        }

        // Если индекс разделения найден, разделяем строку
        if (splitIndex > 0)
        {
            string firstName = fullName.Substring(0, splitIndex);
            string lastName = fullName.Substring(splitIndex);
            return (firstName, lastName);
        }

        // Если фамилию не удалось выделить, вернуть всё как имя, фамилия будет пустой
        return (fullName, string.Empty);
    }
}
