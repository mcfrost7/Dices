using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [SerializeField] private List<MapNodeData> _mapNodes = new(); // Хранение нодов карты
    [SerializeField] private List<NewUnitStats> _playerUnits = new(); // Список юнитов игрока
    [SerializeField] private List<NewUnitStats> _unitsStorage = new(); // Список хранилища юнитов
    [SerializeField] private List<ItemConfig> _items = new(); // Список предметов игрока
    [SerializeField] private List<ResourceData> _resources = new(); // Список ресурсов

    public List<MapNodeData> MapNodes { get => _mapNodes; set => _mapNodes = value; }
    public List<NewUnitStats> PlayerUnits { get => _playerUnits; set => _playerUnits = value; }
    public List<NewUnitStats> UnitsStorage { get => _unitsStorage; set => _unitsStorage = value; }
    public List<ItemConfig> Items { get => _items; set => _items = value; }
    public List<ResourceData> Resources { get => _resources; set => _resources = value; }
}

// Сериализуемый класс для хранения данных о ноде карты
[Serializable]
public class MapNodeData
{
    public Vector2 Position;
    public TileType TileType;
    public int LayerIndex;
    public string LocationConfigId;
    public string TileConfigId; // Добавлено для сохранения идентификатора конфига тайла
    public bool IsVisited;
    public bool IsAvailable;

    public MapNodeData(Vector2 position, TileType tileType, int layerIndex, string locationConfigId)
    {
        Position = position;
        TileType = tileType;
        LayerIndex = layerIndex;
        LocationConfigId = locationConfigId;
    }
}