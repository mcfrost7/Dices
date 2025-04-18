using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using Unity.VisualScripting.FullSerializer;

public class CanvasMapGenerator : MonoBehaviour
{
    [System.Serializable]
    public class MapGenerationSettings
    {
        [Header("Map Layout")]
        public RectTransform mapContainer;
        public TileLogic nodePrefab;
        public int numberOfLayers = 5;
        public int minNodesPerLayer = 3;
        public int maxNodesPerLayer = 7;
        public float layerHeight = 150f;
        public List<LocationConfig> locationConfigs;

        [Header("Randomization")]
        public float nodeHorizontalSpread = 50f;
        public List<TileSpawnProbability> tileSpawnProbabilities;

        [Header("Appearance")]
        public Color availableNodeColor = Color.white;
        public Color visitedNodeColor = new Color(0.7f, 0.7f, 0.7f);
        public Color unavailableNodeColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }

    [System.Serializable]
    public class TileSpawnProbability
    {
        public TileType tileType;
        public float probability;
    }

    [System.Serializable]
    public class MapNode
    {
        public TileLogic nodeObject;
        public RectTransform rectTransform;
        public LocationConfig locationConfig;
        public TileType tileType;
        public NewTileConfig tileConfig;
        public int layerIndex;
        public bool isVisited;
        public bool isAvailable;
    }

    public MapGenerationSettings generationSettings;
    public MapNode _currentNode;
    private List<List<MapNode>> layers = new List<List<MapNode>>();
    private int currentAvailableLayer;

    // Событие для кликов по тайлам
    private UnityEvent<MapNode> OnTileClicked;

    private void Start()
    {
        // Make sure the event is initialized before adding listeners
        if (OnTileClicked == null)
            OnTileClicked = new UnityEvent<MapNode>();

        OnTileClicked.AddListener(HandleTileClicked);
    }

    // Генерация карты
    public void GenerateMap()
    {
        ClearExistingNodes();
        layers.Clear();
        float currentLayerY = generationSettings.mapContainer.rect.height / 2;
        bool bossPlaced = false;

        for (int layerIndex = 0; layerIndex < generationSettings.numberOfLayers; layerIndex++)
        {
            int nodesInLayer = layerIndex == 0 ? 1 : Random.Range(generationSettings.minNodesPerLayer, generationSettings.maxNodesPerLayer + 1);

            List<MapNode> layerNodes = CreateNodesForLayer(layerIndex, currentLayerY, nodesInLayer, ref bossPlaced);
            layers.Add(layerNodes);
            currentLayerY -= generationSettings.layerHeight;
        }

        SetInitialAvailableLayer();
    }

    // Устанавливаем начальный доступный слой
    private void SetInitialAvailableLayer()
    {
        currentAvailableLayer = layers.Count - 1;
        UpdateNodesAvailability();
    }

    // Обновляем доступность узлов
    private void UpdateNodesAvailability()
    {
        foreach (var layer in layers)
        {
            foreach (var node in layer)
            {
                node.isAvailable = false;
                UpdateNodeVisuals(node);
            }
        }

        if (currentAvailableLayer < 0) return;

        foreach (var node in layers[currentAvailableLayer])
        {
            if (!node.isVisited)
            {
                node.isAvailable = true;
                UpdateNodeVisuals(node);
            }
        }
    }

    // Обновляем визуальные изменения узлов
    private void UpdateNodeVisuals(MapNode node)
    {
        if (node.isVisited)
        {
            node.nodeObject.Image.color = generationSettings.visitedNodeColor;
            node.nodeObject.Button.interactable = false;
        }
        else if (node.isAvailable)
        {
            node.nodeObject.Image.color = generationSettings.availableNodeColor;
            node.nodeObject.Button.interactable = true;
        }
        else
        {
            node.nodeObject.Image.color = generationSettings.unavailableNodeColor;
            node.nodeObject.Button.interactable = false;
        }
    }

    // Загружаем карту из сохраненных данных
    public void LoadMapFromPlayerData(PlayerData playerData)
    {
        if (playerData.MapNodes == null || playerData.MapNodes.Count == 0)
        {
            Debug.Log("No saved map data. Generating new map.");
            GenerateMap();
            return;
        }

        ClearExistingNodes();
        layers.Clear();

        var nodesByLayer = playerData.MapNodes.GroupBy(n => n.LayerIndex).OrderBy(g => g.Key);

        foreach (var layerGroup in nodesByLayer)
        {
            List<MapNode> layerNodes = new List<MapNode>();

            foreach (MapNodeData nodeData in layerGroup)
            {
                LocationConfig locationConfig = generationSettings.locationConfigs.FirstOrDefault(config => config.name == nodeData.LocationConfigId) ?? generationSettings.locationConfigs.FirstOrDefault();

                if (locationConfig == null)
                {
                    Debug.LogWarning($"Location config with ID {nodeData.LocationConfigId} not found!");
                    continue;
                }

                TileLogic nodeObject = Instantiate(generationSettings.nodePrefab, generationSettings.mapContainer);
                RectTransform nodeRectTransform = nodeObject.GetComponent<RectTransform>();
                nodeRectTransform.anchoredPosition = nodeData.Position;

                LocationConfig.LocationTile tileConfig = locationConfig.tiles.FirstOrDefault(tile => tile.tileConfig.tileType == nodeData.TileType) ?? locationConfig.tiles.FirstOrDefault();

                if (tileConfig == null)
                {
                    Debug.LogWarning($"No tile configurations found in location {locationConfig.name}!");
                    continue;
                }

                MapNode mapNode = new MapNode
                {
                    nodeObject = nodeObject,
                    rectTransform = nodeRectTransform,
                    locationConfig = locationConfig,
                    tileType = nodeData.TileType,
                    tileConfig = tileConfig.tileConfig,
                    layerIndex = nodeData.LayerIndex,
                    isVisited = nodeData.IsVisited,
                    isAvailable = nodeData.IsAvailable
                };

                nodeObject.Initialize(tileConfig.tileConfig.tileSprite, OnNodeClick, mapNode);
                layerNodes.Add(mapNode);
            }

            layers.Add(layerNodes);
        }

        DetermineCurrentAvailableLayer();
        UpdateNodesAvailability();
        _currentNode = FindLastVisitedNode();
        if (_currentNode != null)
        {
            if (_currentNode.tileConfig.tileType == TileType.CampTile)
            {
                CampPanel.Instance.SetupInfo(_currentNode.tileConfig);
            }
        }
    }

    private MapNode FindLastVisitedNode()
    {
        for (int i = 0; i < layers.Count; i++)
        {
            List<MapNode> visitedNodes = layers[i].Where(node => node.isVisited).ToList();

            if (visitedNodes.Count > 0)
            {
                return visitedNodes.Last();
            }
        }
        return null;
    }

    private void DetermineCurrentAvailableLayer()
    {
        currentAvailableLayer = layers.Count - 1;

        for (int i = layers.Count - 1; i >= 0; i--)
        {
            if (layers[i].Any(node => node.isVisited))
            {
                if (i > 0 && layers[i - 1].All(node => !node.isVisited))
                {
                    currentAvailableLayer = i - 1;
                    break;
                }
            }
        }
    }

    public void SaveMapToPlayerData()
    {
        List<MapNodeData> mapNodesData = new List<MapNodeData>();

        for (int layerIndex = 0; layerIndex < layers.Count; layerIndex++)
        {
            foreach (MapNode node in layers[layerIndex])
            {
                MapNodeData nodeData = new MapNodeData(node.rectTransform.anchoredPosition, node.tileType, layerIndex, node.locationConfig.name);
                nodeData.IsVisited = node.isVisited;
                nodeData.IsAvailable = node.isAvailable;
                nodeData.TileConfigId = node.tileConfig.name;
                mapNodesData.Add(nodeData);
            }
        }

        GameDataMNG.Instance.PlayerData.MapNodes = mapNodesData;
    }

    // Создаем узлы для слоя
    private List<MapNode> CreateNodesForLayer(int layerIndex, float layerY, int nodesInLayer, ref bool bossPlaced)
    {
        List<MapNode> layerNodes = new List<MapNode>();
        float totalWidth = generationSettings.mapContainer.rect.width;
        float spacing = totalWidth / (nodesInLayer + 1);

        for (int i = 0; i < nodesInLayer; i++)
        {
            TileLogic nodeObject = Instantiate(generationSettings.nodePrefab, generationSettings.mapContainer);
            RectTransform nodeRectTransform = nodeObject.GetComponent<RectTransform>();
            float xPosition = spacing * (i + 1) - totalWidth / 2 + Random.Range(-generationSettings.nodeHorizontalSpread, generationSettings.nodeHorizontalSpread);
            nodeRectTransform.anchoredPosition = new Vector2(xPosition, layerY);
            LocationConfig selectedLocationConfig = null;

            // Handle boss placement on the first layer
            if (!bossPlaced && layerIndex == 0)
            {
                selectedLocationConfig = generationSettings.locationConfigs[Random.Range(0, generationSettings.locationConfigs.Count)];

                if (selectedLocationConfig != null)
                {
                    LocationConfig.LocationTile bossTile = selectedLocationConfig.tiles.FirstOrDefault(tile => tile.tileConfig.tileType == TileType.BossTile);

                    if (bossTile != null)
                    {
                        MapNode newNode = new MapNode
                        {
                            nodeObject = nodeObject,
                            rectTransform = nodeRectTransform,
                            locationConfig = selectedLocationConfig,
                            tileType = TileType.BossTile,
                            tileConfig = bossTile.tileConfig,
                            layerIndex = layerIndex,
                            isVisited = false,
                            isAvailable = false
                        };

                        newNode.nodeObject.Initialize(bossTile.tileConfig.tileSprite, OnNodeClick, newNode);
                        layerNodes.Add(newNode);
                        bossPlaced = true;
                        continue;
                    }
                }
            }

            // Create regular tile
            TileType selectedTileType = GetRandomTileType();
            selectedLocationConfig = generationSettings.locationConfigs[Random.Range(0, generationSettings.locationConfigs.Count)];

            LocationConfig.LocationTile generatedTile = selectedLocationConfig.tiles.FirstOrDefault(tile => tile.tileConfig.tileType == selectedTileType);

            if (generatedTile == null)
            {
                generatedTile = selectedLocationConfig.tiles.FirstOrDefault();
                selectedTileType = generatedTile?.tileConfig.tileType ?? TileType.BattleTile;
            }

            MapNode newTileNode = new MapNode
            {
                nodeObject = nodeObject,
                rectTransform = nodeRectTransform,
                locationConfig = selectedLocationConfig,
                tileType = selectedTileType,
                tileConfig = generatedTile.tileConfig,
                layerIndex = layerIndex,
                isVisited = false,
                isAvailable = false
            };

            newTileNode.nodeObject.Initialize(generatedTile.tileConfig.tileSprite, OnNodeClick, newTileNode);
            layerNodes.Add(newTileNode);
        }

        return layerNodes;
    }

    private void HandleTileClicked(MapNode node)
    {
        if (!node.isAvailable) return;

        node.isVisited = true;
        node.isAvailable = false;

        foreach (var layerNode in layers[currentAvailableLayer])
        {
            layerNode.isAvailable = false;
        }

        bool allNodesInLayerVisited = layers[currentAvailableLayer].All(n => n.isVisited);
        if (!allNodesInLayerVisited)
        {
            UpdateNodesAvailability();
        }

        currentAvailableLayer--;

        if (currentAvailableLayer < 0)
        {
            Debug.Log("Map fully completed!");
        }
        else
        {
            UpdateNodesAvailability();
        }
        SaveMapToPlayerData();
        GameDataMNG.Instance.HandleTileClick(node);
    }

    // Обработчик клика по узлу
    public void OnNodeClick(MapNode node)
    {
        OnTileClicked?.Invoke(node);
    }

    // Получаем случайный тип тайла на основе вероятности
    private TileType GetRandomTileType()
    {
        float totalProbability = generationSettings.tileSpawnProbabilities.Sum(t => t.probability);

        if (totalProbability <= 0f)
        {
            Debug.LogWarning("All tile spawn probabilities are 0. Returning default BattleTile.");
            return TileType.BattleTile;
        }

        if (totalProbability < 1f)
        {
            float normalizationFactor = 1f / totalProbability;
            foreach (var item in generationSettings.tileSpawnProbabilities)
            {
                item.probability *= normalizationFactor;
            }
            totalProbability = 1f;
        }

        float randomValue = Random.Range(0f, totalProbability);

        foreach (TileSpawnProbability tileProb in generationSettings.tileSpawnProbabilities)
        {
            randomValue -= tileProb.probability;
            if (randomValue <= 0f)
            {
                return tileProb.tileType;
            }
        }

        return TileType.BattleTile; // Default
    }

    // Очищаем старые узлы карты
    private void ClearExistingNodes()
    {
        foreach (Transform child in generationSettings.mapContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
