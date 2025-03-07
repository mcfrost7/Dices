using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [SerializeField] private List<MapNodeData> mapNodes = new(); // Хранение нодов карты
    [SerializeField] private int currentAvailableLayer = -1; // Текущий доступный слой
    [SerializeField] private List<NewUnitStats> playerUnits = new(); // Список юнитов игрока

    public List<MapNodeData> MapNodes { get => mapNodes; set => mapNodes = value; }
    public int CurrentAvailableLayer { get => currentAvailableLayer; set => currentAvailableLayer = value; }
    public List<NewUnitStats> PlayerUnits { get => playerUnits; set => playerUnits = value; }
}

// Сериализуемый класс для хранения данных о ноде карты
[Serializable]
public class MapNodeData
{
    [SerializeField] private Vector2 position; // Позиция нода
    [SerializeField] private TileType tileType; // Тип тайла
    [SerializeField] private int layerIndex; // Индекс слоя
    [SerializeField] private string locationConfigId; // ID конфигурации локации
    [SerializeField] private bool isVisited = false; // Флаг посещения
    [SerializeField] private bool isAvailable = false; // Флаг доступности для посещения

    public Vector2 Position { get => position; set => position = value; }
    public TileType TileType { get => tileType; set => tileType = value; }
    public int LayerIndex { get => layerIndex; set => layerIndex = value; }
    public string LocationConfigId { get => locationConfigId; set => locationConfigId = value; }
    public bool IsVisited { get => isVisited; set => isVisited = value; }
    public bool IsAvailable { get => isAvailable; set => isAvailable = value; }

    // Конструктор для удобства создания
    public MapNodeData(Vector2 position, TileType tileType, int layerIndex, string locationConfigId)
    {
        this.position = position;
        this.tileType = tileType;
        this.layerIndex = layerIndex;
        this.locationConfigId = locationConfigId;
    }
}