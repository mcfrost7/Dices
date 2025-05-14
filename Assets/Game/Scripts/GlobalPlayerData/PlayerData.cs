using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[Serializable]
public class PlayerData
{
    private List<MapNodeData> _mapNodes = new();
    private List<NewUnitStats> _playerUnits = new();
    private List<NewUnitStats> _unitsStorage = new();
    private List<ItemInstance> _items = new();
    private List<ResourceData> _resourcesData = new();
    private List<LocationConfig> _locationsConfigs = new();
    private int _locationLevel = 1;
    private bool _isTutorialTeam = true;
    private bool _isTutorialMap = true;
    private bool _isTutorialBattle = true;
    private bool _isTutorialNeeded = true;

    public List<MapNodeData> MapNodes { get => _mapNodes; set => _mapNodes = value; }
    public List<NewUnitStats> PlayerUnits { get => _playerUnits; set => _playerUnits = value; }
    public List<NewUnitStats> UnitsStorage { get => _unitsStorage; set => _unitsStorage = value; }
    public List<ItemInstance> Items { get => _items; set => _items = value; }
    public List<ResourceData> ResourcesData { get => _resourcesData; set => _resourcesData = value; }
    public int LocationLevel { get => _locationLevel; set => _locationLevel = value; }
    public List<LocationConfig> LocationsConfigs { get => _locationsConfigs; set => _locationsConfigs = value; }
    public bool IsTutorialTeam { get => _isTutorialTeam; set => _isTutorialTeam = value; }
    public bool IsTutorialMap { get => _isTutorialMap; set => _isTutorialMap = value; }
    public bool IsTutorialBattle { get => _isTutorialBattle; set => _isTutorialBattle = value; }
    public bool IsTutorialNeeded { get => _isTutorialNeeded; set => _isTutorialNeeded = value; }
}
[Serializable]
public class SerializablePlayerData
{
    public List<MapNodeData> MapNodes = new List<MapNodeData>();
    public List<SerializableUnitStats> PlayerUnits = new List<SerializableUnitStats>();
    public List<SerializableUnitStats> UnitsStorage = new List<SerializableUnitStats>();
    public List<SerializableResourceData> ResourcesData = new List<SerializableResourceData>();
    public List<SerializableItemConfig> Items = new List<SerializableItemConfig>();
    public List<SerializableLocation> Locations = new List<SerializableLocation>();

    public int LocationLevel;
    public bool IsTutorialTeam;
    public bool IsTutorialMap;
    public bool IsTutorialBattle;
    public bool IsTutorialNeeded;
    // Конструктор для создания из PlayerData
    public SerializablePlayerData(PlayerData data)
    {
        // Копируем узлы карты
        if (data.MapNodes != null)
        {
            MapNodes = new List<MapNodeData>(data.MapNodes);
        }
        if (data.PlayerUnits != null)
        {
            foreach (var unit in data.PlayerUnits)
            {
                PlayerUnits.Add(new SerializableUnitStats(unit));
            }
        }
        if (data.UnitsStorage != null)
        {
            foreach (var unit in data.UnitsStorage)
            {
                UnitsStorage.Add(new SerializableUnitStats(unit));
            }
        }

        if (data.ResourcesData != null && data.ResourcesData.Count > 0)
        {
            // Используем словарь для группировки ресурсов по ID
            Dictionary<string, SerializableResourceData> resourcesDict = new Dictionary<string, SerializableResourceData>();

            foreach (var resource in data.ResourcesData)
            {
                SerializableResourceData serResource = resource.ToSerializable();
                string configId = serResource.ConfigID;

                if (resourcesDict.ContainsKey(configId))
                {
                    // Если такой ресурс уже есть, суммируем количество
                    resourcesDict[configId].Count += serResource.Count;
                }
                else
                {
                    // Если такого ресурса еще нет, добавляем
                    resourcesDict[configId] = serResource;
                }
            }

            // Конвертируем словарь обратно в список
            ResourcesData = new List<SerializableResourceData>(resourcesDict.Values);

        }
        if (data.Items != null && data.Items.Count > 0)
        {
            foreach (var item in data.Items)
            {
                Items.Add(new SerializableItemConfig(item));
            }
        }
        LocationLevel = data.LocationLevel;
        IsTutorialTeam = data.IsTutorialTeam;
        IsTutorialBattle = data.IsTutorialBattle;
        IsTutorialNeeded = data.IsTutorialNeeded;
        IsTutorialMap = data.IsTutorialMap;
        if (data.LocationsConfigs != null && data.LocationsConfigs.Count > 0)
        {
            foreach (var locationConfig in data.LocationsConfigs)
            {
                Locations.Add(new SerializableLocation(locationConfig));
            }
        }
    }

    // Метод для конвертации обратно в PlayerData
    public PlayerData ToPlayerData()
    {
        PlayerData data = new PlayerData();
        data.LocationLevel = LocationLevel;
        data.IsTutorialTeam = IsTutorialTeam;
        data.IsTutorialBattle = IsTutorialBattle;
        data.IsTutorialMap = IsTutorialMap;
        data.IsTutorialNeeded = IsTutorialNeeded;
        // Копируем узлы карты
        data.MapNodes = new List<MapNodeData>(MapNodes);

        // Конвертируем юниты игрока
        data.PlayerUnits = new List<NewUnitStats>();
        foreach (var serUnit in PlayerUnits)
        {
            data.PlayerUnits.Add(serUnit.ToUnitStats());
        }

        // Конвертируем юниты в хранилище
        data.UnitsStorage = new List<NewUnitStats>();
        foreach (var serUnit in UnitsStorage)
        {
            data.UnitsStorage.Add(serUnit.ToUnitStats());
        }
        data.ResourcesData = new List<ResourceData>();

        // Используем словарь для группировки ресурсов по ID
        Dictionary<string, ResourceData> resourcesDict = new Dictionary<string, ResourceData>();

        foreach (var serResource in ResourcesData)
        {
            // Находим конфигурацию ресурса
            ResourceConfig config = Resources.Load<ResourceConfig>(serResource.ConfigID);
            if (config != null)
            {
                ResourceData resourceData = new ResourceData(config, serResource.Count);

                // Используем ID конфига как ключ
                string configId = config.ResourceName;

                if (resourcesDict.ContainsKey(configId))
                {
                    // Если такой ресурс уже есть, суммируем количество
                    resourcesDict[configId].Count += resourceData.Count;
                }
                else
                {
                    // Если такого ресурса еще нет, добавляем
                    resourcesDict[configId] = resourceData;
                }
            }
        }
        data.ResourcesData = new List<ResourceData>(resourcesDict.Values);


        data.Items = new List<ItemInstance>();
        foreach (var serItem in Items)
        {
            data.Items.Add(serItem.ToItemInstance());
        }
        data.LocationsConfigs = new List<LocationConfig>();
        foreach (var serLocation in Locations)
        {
            var loc = Resources.Load<LocationConfig>("Configs/MapElements/" + serLocation._configName);
            if (loc != null)
            {
                data.LocationsConfigs.Add(loc);
            }
        }

        return data;
    }
}
[Serializable]
public class MapNodeData
{
    public Vector2 Position;
    public TileType TileType;
    public int LayerIndex;
    public string LocationConfigId;
    public string TileConfigId;
    public bool IsVisited;
    public bool IsAvailable;
    public int battleDifficulty;

    public MapNodeData() { } // Default constructor for serialization

    public MapNodeData(Vector2 position, TileType tileType, int layerIndex, string locationConfigId)
    {
        Position = position;
        TileType = tileType;
        LayerIndex = layerIndex;
        LocationConfigId = locationConfigId;
    }
}