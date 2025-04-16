using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [SerializeField] private List<MapNodeData> _mapNodes = new();
    [SerializeField] private List<NewUnitStats> _playerUnits = new();
    [SerializeField] private List<NewUnitStats> _unitsStorage = new();
    [SerializeField] private List<ItemInstance> _items = new();
    [SerializeField] private List<ResourceData> _resourcesData = new();

    public List<MapNodeData> MapNodes { get => _mapNodes; set => _mapNodes = value; }
    public List<NewUnitStats> PlayerUnits { get => _playerUnits; set => _playerUnits = value; }
    public List<NewUnitStats> UnitsStorage { get => _unitsStorage; set => _unitsStorage = value; }
    public List<ItemInstance> Items { get => _items; set => _items = value; }
    public List<ResourceData> ResourcesData { get => _resourcesData; set => _resourcesData = value; }

}
[Serializable]
public class SerializablePlayerData
{
    public List<MapNodeData> MapNodes = new List<MapNodeData>();
    public List<SerializableUnitStats> PlayerUnits = new List<SerializableUnitStats>();
    public List<SerializableUnitStats> UnitsStorage = new List<SerializableUnitStats>();
    public List<SerializableResourceData> ResourcesData = new List<SerializableResourceData>();
    public List<SerializableItemConfig> Items = new List<SerializableItemConfig>();

    // ����������� ��� �������� �� PlayerData
    public SerializablePlayerData(PlayerData data)
    {
        // �������� ���� �����
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
            // ���������� ������� ��� ����������� �������� �� ID
            Dictionary<string, SerializableResourceData> resourcesDict = new Dictionary<string, SerializableResourceData>();

            foreach (var resource in data.ResourcesData)
            {
                SerializableResourceData serResource = resource.ToSerializable();
                string configId = serResource.ConfigID;

                if (resourcesDict.ContainsKey(configId))
                {
                    // ���� ����� ������ ��� ����, ��������� ����������
                    resourcesDict[configId].Count += serResource.Count;
                }
                else
                {
                    // ���� ������ ������� ��� ���, ���������
                    resourcesDict[configId] = serResource;
                }
            }

            // ������������ ������� ������� � ������
            ResourcesData = new List<SerializableResourceData>(resourcesDict.Values);

        }
        if (data.Items != null)
        {
            foreach (var item in data.Items)
            {
                Items.Add(new SerializableItemConfig(item));
            }
        }
    }

    // ����� ��� ����������� ������� � PlayerData
    public PlayerData ToPlayerData()
    {
        PlayerData data = new PlayerData();

        // �������� ���� �����
        data.MapNodes = new List<MapNodeData>(MapNodes);

        // ������������ ����� ������
        data.PlayerUnits = new List<NewUnitStats>();
        foreach (var serUnit in PlayerUnits)
        {
            data.PlayerUnits.Add(serUnit.ToUnitStats());
        }

        // ������������ ����� � ���������
        data.UnitsStorage = new List<NewUnitStats>();
        foreach (var serUnit in UnitsStorage)
        {
            data.UnitsStorage.Add(serUnit.ToUnitStats());
        }
        data.ResourcesData = new List<ResourceData>();

        // ���������� ������� ��� ����������� �������� �� ID
        Dictionary<string, ResourceData> resourcesDict = new Dictionary<string, ResourceData>();

        foreach (var serResource in ResourcesData)
        {
            // ������� ������������ �������
            ResourceConfig config = Resources.Load<ResourceConfig>(serResource.ConfigID);
            if (config != null)
            {
                ResourceData resourceData = new ResourceData(config, serResource.Count);

                // ���������� ID ������� ��� ����
                string configId = config.ResourceName;

                if (resourcesDict.ContainsKey(configId))
                {
                    // ���� ����� ������ ��� ����, ��������� ����������
                    resourcesDict[configId].Count += resourceData.Count;
                }
                else
                {
                    // ���� ������ ������� ��� ���, ���������
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

    public MapNodeData() { } // Default constructor for serialization

    public MapNodeData(Vector2 position, TileType tileType, int layerIndex, string locationConfigId)
    {
        Position = position;
        TileType = tileType;
        LayerIndex = layerIndex;
        LocationConfigId = locationConfigId;
    }
}