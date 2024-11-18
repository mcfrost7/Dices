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
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private TypesInfo typesInfo;

    public Player TeamPlayer { get => teamPlayer; set => teamPlayer = value; }
    public Canvas Canvas1 { get => Canvas; set => Canvas = value; }
    public GameObject TeamPlace { get => teamPlace; set => teamPlace = value; }
    public GameObject UnitPrefab { get => unitPrefab; set => unitPrefab = value; }
    public TypesInfo TypesInfo { get => typesInfo; set => typesInfo = value; }

    private void OnEnable()
    {
        Canvas1.gameObject.SetActive(true);
        TeamPlayer = GameManager.Instance.Player;
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
        if (Canvas1 != null)
            Canvas1.gameObject.SetActive(false);    
    }


    public void CreateRandomTeam()
    {
        for (int i = 0; i < 5; i++)
        {
            // Генерация случайных значений для параметров
            int randomHealth = Random.Range(4, 6);
            int randomMoral = Random.Range(2, 3);
            TypesInfo.Type randomType = TypesInfo.types[Random.Range(0, TypesInfo.types.Length)];
            GameManager.Instance.Player.units.Add(new UnitStats(randomHealth, randomMoral, randomType)); 
        }
    }

    private void DrawTeam()
    {
        Button[] buttons = TeamPlace.GetComponentsInChildren<Button>();
        int index = 0;

        // Проходим по кнопкам и юнитам одновременно
        foreach (Button button in buttons)
        {
            if (index < TeamPlayer.units.Count)
            {
                // Устанавливаем изображение для кнопки
                Image buttonImage = button.GetComponent<Image>();
                buttonImage.sprite = TeamPlayer.units[index].Sprite;  // Берём спрайт юнита

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
        UnitStats selectedUnit = TeamPlayer.units[unitIndex];
        Debug.Log("Выбран юнит с показателями: " +
                  "Текущее здоровье: " + selectedUnit.CurrentHealth +
                  ", Мораль: " + selectedUnit.Moral +
                  ", Тип юнита: " + selectedUnit.Type.TypeName);

    }
}
