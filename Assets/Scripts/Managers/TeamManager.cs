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
        for (int i = 0; i < 5; i++)
        {
            // Генерация случайных значений для параметров
            int randomHealth = Random.Range(4, 6);
            int randomMoral = Random.Range(2, 3);
            TypesInfo.Type randomType = typesInfo.types[Random.Range(0, typesInfo.types.Length)];
            GameManager.Instance.player.units.Add(new UnitStats(randomHealth, randomMoral, randomType)); 
        }
    }

    private void DrawTeam()
    {
        Button[] buttons = teamPlace.GetComponentsInChildren<Button>();
        int index = 0;

        // Проходим по кнопкам и юнитам одновременно
        foreach (Button button in buttons)
        {
            if (index < teamPlayer.units.Count)
            {
                // Устанавливаем изображение для кнопки
                Image buttonImage = button.GetComponent<Image>();
                buttonImage.sprite = teamPlayer.units[index].Sprite;  // Берём спрайт юнита

                // Удаляем существующие события и добавляем новое
                button.onClick.RemoveAllListeners();
                int unitIndex = index; // Сохраняем индекс, чтобы избежать замыкания лямбда-выражения
                button.onClick.AddListener(() => OnUnitButtonClicked(unitIndex));
            }
            index++;
        }
    }


    private void OnUnitButtonClicked(int unitIndex)
    {
        UnitStats selectedUnit = teamPlayer.units[unitIndex];
        Debug.Log("Выбран юнит с показателями: " +
                  "Текущее здоровье: " + selectedUnit.CurrentHealth +
                  ", Мораль: " + selectedUnit.Moral +
                  ", Тип юнита: " + selectedUnit.Type.TypeName);

    }
}
