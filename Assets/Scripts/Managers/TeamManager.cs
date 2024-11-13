using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TeamManager : MonoBehaviour
{
    Player teamPlayer = null;
    [SerializeField] private Canvas Canvas;
    [SerializeField] private GameObject teamPlace;
    [SerializeField] private TypesInfo typesInfo;
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
    private void Awake()
    {
        gameObject.SetActive(false);
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
            int randomHealth = Random.Range(4, 6);
            int randomMoral = Random.Range(2, 3);
            int randomInventory = Random.Range(1, 3);
            TypesInfo.Type randomType = typesInfo.types[Random.Range(0, typesInfo.types.Length)];
            teamPlayer.units[i].Init(randomHealth, randomMoral ,randomInventory,randomType);
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
                buttonImage.sprite = teamPlayer.units[i].Sprite;  // Берём спрайт юнита

                // Сохраняем индекс, чтобы избежать замыкания лямбда-выражения
                int unitIndex = i;

                // Удаляем существующие события и добавляем новое
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => OnUnitButtonClicked(unitIndex));
            }
        }
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
