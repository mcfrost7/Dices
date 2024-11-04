using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamManager : MonoBehaviour
{
    private Unit[] team = new Unit[5];
    [SerializeField] private Dice diceGameobject;
    [SerializeField] private Canvas Canvas;
    [SerializeField] private Sprite[] sprites;

    [SerializeField] private GameObject teamPlace;

    private void OnEnable()
    {
        Canvas.gameObject.SetActive(true);
        CreateRandomTeam();
        DrawTeam();
    }

    private void OnDisable()
    {   
        if (Canvas != null)
            Canvas.gameObject.SetActive(false);    
    }

    public void CreateRandomTeam()
    {
        for (int i = 0; i < 5; i++)
        {
            team[i] = new Unit();   
            int randomHealth = Random.Range(1, 6);   
            int randomMoral = Random.Range(1, 3);      
            int randomInventory = Random.Range(1, 3);
            Dice dice = diceGameobject;
            int randomSprite = Random.Range(0, sprites.Length);
            team[i].Init(randomHealth, randomMoral, randomInventory, dice, sprites[randomSprite]);
         }

    }
    private void DrawTeam()
    {
        Button[] buttons = teamPlace.GetComponentsInChildren<Button>();

        for (int i = 0; i < team.Length; i++)
        {
            if (i < buttons.Length)
            {
                int j = i + 1;
                // Устанавливаем изображение для кнопки
                Image buttonImage = buttons[j].GetComponent<Image>();
                buttonImage.sprite = team[i].UnitSprite;  // Берём спрайт юнита

                // Сохраняем индекс, чтобы избежать замыкания лямбда-выражения
                int unitIndex = i;

                // Удаляем существующие события и добавляем новое
                buttons[j].onClick.RemoveAllListeners();
                buttons[j].onClick.AddListener(() => OnUnitButtonClicked(unitIndex));
            }
        }
    }

    private void OnUnitButtonClicked(int unitIndex)
    {
        Unit selectedUnit = team[unitIndex];
        Debug.Log("Выбран юнит с показателями: " +
                  "Здоровье: " + selectedUnit.Health +
                  ", Мораль: " + selectedUnit.Moral +
                  ", Инвентарь: " + selectedUnit.Inventory);

    }
}
