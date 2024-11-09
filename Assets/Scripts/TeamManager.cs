using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TeamManager : MonoBehaviour
{
    Player teamPlayer = null;
    [SerializeField] private Canvas Canvas;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private string[] types;
    [SerializeField] private GameObject teamPlace;
    [SerializeField] private Sprite[] diceSprite;

    private void OnEnable()
    {
        Canvas.gameObject.SetActive(true);
        teamPlayer = GameManager.Instance.player;
        if (GameManager.Instance.GetGameStatus() == true)
        {
            CreateRandomTeam();
            GameManager.Instance.SetGameStatus(false);
        }
        DrawTeam();
    }

    private void OnDisable()
    {   
        if (Canvas != null)
            Canvas.gameObject.SetActive(false);    
    }


    public void CreateRandomTeam()
    {
        // Убедитесь, что массив был создан и инициализирован объектами Unit
        if (teamPlayer.units == null || teamPlayer.units.Length != 5)
        {
            teamPlayer.units = new Unit[5];

            for (int i = 0; i < teamPlayer.units.Length; i++)
            {
                teamPlayer.units[i] = new Unit(); // Инициализируем каждый элемент массива объектом Unit
            }
        }

        for (int i = 0; i < teamPlayer.units.Length; i++)
        {
            int randomHealth = Random.Range(1, 6);
            int randomMoral = Random.Range(1, 3);
            int randomInventory = Random.Range(1, 3);
            int randomType = Random.Range(1, types.Length);
            string randomTypeName = types[randomType];
            teamPlayer.units[i].Init(randomHealth, randomMoral, diceSprite ,randomInventory, randomTypeName, GetSpriteByType(randomTypeName));
        }
    }

    private void DrawTeam()
    {
        Button[] buttons = teamPlace.GetComponentsInChildren<Button>();

        for (int i = 0; i < 5; i++)
        {
            if (i < buttons.Length)
            {
                // Устанавливаем изображение для кнопки
                Image buttonImage = buttons[i].GetComponent<Image>();
                buttonImage.sprite = GetSpriteByType(teamPlayer.units[i].Type);  // Берём спрайт юнита

                // Сохраняем индекс, чтобы избежать замыкания лямбда-выражения
                int unitIndex = i;

                // Удаляем существующие события и добавляем новое
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => OnUnitButtonClicked(unitIndex));
            }
        }
    }



    private Sprite GetSpriteByType(string type)
    {
        Sprite sprite = sprites[0];
        for (int i = 0; i < types.Length; i++)
        {
            if (types[i] ==  type)
                return sprites[i];
        }
        return sprite;

    }

    private void OnUnitButtonClicked(int unitIndex)
    {
        Unit selectedUnit = teamPlayer.units[unitIndex];
        Debug.Log("Выбран юнит с показателями: " +
                  "Здоровье: " + selectedUnit.Health +
                  ", Мораль: " + selectedUnit.Moral +
                  ", Инвентарь: " + selectedUnit.Inventory);

    }
}
