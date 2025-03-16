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
    [SerializeField] private List<ItemConfig> _items = new();
    [SerializeField] private List<SerializableResourceData> _resourcesData = new(); // Changed to SerializableResourceData

    // Fixed property syntax (removed * symbols)
    public List<MapNodeData> MapNodes { get => _mapNodes; set => _mapNodes = value; }
    public List<NewUnitStats> PlayerUnits { get => _playerUnits; set => _playerUnits = value; }
    public List<NewUnitStats> UnitsStorage { get => _unitsStorage; set => _unitsStorage = value; }
    public List<ItemConfig> Items { get => _items; set => _items = value; }
    public List<SerializableResourceData> ResourcesData { get => _resourcesData; set => _resourcesData = value; }

    // For runtime use, not serialized
    [NonSerialized]
    public List<ResourceData> Resources = new();
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