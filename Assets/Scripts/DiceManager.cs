using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    private List<Dice> diceList = new List<Dice>();

    private void Start()
    {
        // Получаем все компоненты Dice в сцене
        Dice[] diceComponents = FindObjectsOfType<Dice>();
        diceList.AddRange(diceComponents);
    }

    // Метод для вызова RollDice у всех Dice
    public void RollAllDices()
    {
        foreach (Dice dice in diceList)
        {
            dice.RollDice(); // Вызываем метод RollDice у каждого кубика
        }
    }
}
