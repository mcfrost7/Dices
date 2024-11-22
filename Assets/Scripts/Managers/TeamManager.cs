using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    private Player teamPlayer = null;

    [SerializeField] private TypesInfo typesInfo;

    [SerializeField] private GameObject unitPrefab;

    public Player TeamPlayer { get => teamPlayer; private set => teamPlayer = value; }
    public TypesInfo TypesInfo { get => typesInfo; set => typesInfo = value; }

    public GameObject UnitPrefab { get => unitPrefab; set => unitPrefab = value; }

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void CreateUnit(int numberOfUnits)
    {
        for (int i = 0; i < numberOfUnits; i++)
        {
            // Генерация случайных значений для параметров
            int randomHealth = Random.Range(3, 8);
            int randomMoral = Random.Range(1, 4);
            TypesInfo.Type randomType = TypesInfo.types[Random.Range(0, TypesInfo.types.Length)];
            GameManager.Instance.Player.Units.Add(new UnitStats(randomHealth, randomMoral, randomType));
        }
    }



    public List<UnitStats> GetPlayerUnits()
    {
        return GameManager.Instance.Player.Units;
    }
}
