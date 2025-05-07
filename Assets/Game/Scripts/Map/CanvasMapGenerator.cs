using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using System;
using Random = UnityEngine.Random;

public class CanvasMapGenerator : MonoBehaviour
{
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
        public int battleDifficulty;
    }

    public RectTransform mapContainer;
    public TileLogic nodePrefab;
    public List<LocationConfig> locationConfigs;
    private MapNode currentNode;
    private List<List<MapNode>> layers = new List<List<MapNode>>();
    private int currentAvailableLayer;
    private MapGenerationSettings generationSettings = new MapGenerationSettings();
    private LocationConfig selectedLocationConfig;
    private UnityEvent<MapNode> OnTileClicked;

    public MapNode CurrentNode { get => currentNode; set => currentNode = value; }
    public LocationConfig SelectedLocationConfig { get => selectedLocationConfig; set => selectedLocationConfig = value; }

    private void Start()
    {
        // Make sure the event is initialized before adding listeners
        if (OnTileClicked == null)
            OnTileClicked = new UnityEvent<MapNode>();
        OnTileClicked.AddListener(HandleTileClicked);
    }

    public void GenerateMap(int level)
    {
        ClearExistingNodes();
        layers.Clear();
        SelectedLocationConfig = GetRandomLocationForLevel(level);
        GameDataMNG.Instance.PlayerData.LocationsConfigs = locationConfigs;
        SetupGeneratorSettings(SelectedLocationConfig);
        float currentLayerY = mapContainer.rect.height / 2;
        bool bossPlaced = false;
        MenuMNG.Instance.SetupLocation(SelectedLocationConfig._locationName);
        for (int layerIndex = 0; layerIndex < generationSettings.numberOfLayers; layerIndex++)
        {
            int nodesInLayer = layerIndex == 0 ? 1 : Random.Range(generationSettings.minNodesPerLayer, generationSettings.maxNodesPerLayer + 1);

            List<MapNode> layerNodes = CreateNodesForLayer(layerIndex, currentLayerY, nodesInLayer, ref bossPlaced, SelectedLocationConfig);
            layers.Add(layerNodes);
            currentLayerY -= generationSettings.layerHeight;
        }
        SetInitialAvailableLayer();
    }

    public LocationConfig GetRandomLocationForLevel(int level)
    {
        var matchingLocations = locationConfigs.Where(loc => loc._locationLevel == level).ToList();

        if (matchingLocations.Count == 0)
        {
            Debug.LogWarning($"Нет локаций с уровнем {level}!");
            return null;
        }
        return matchingLocations[Random.Range(0, matchingLocations.Count)];
    }

    private void SetupGeneratorSettings(LocationConfig selectedLocationConfig)
    {

        generationSettings = selectedLocationConfig.generationSettings;
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
            GenerateMap(playerData.LocationLevel);
            return;
        }

        ClearExistingNodes();
        layers.Clear();

        var nodesByLayer = playerData.MapNodes.GroupBy(n => n.LayerIndex).OrderBy(g => g.Key);
        selectedLocationConfig = playerData.LocationsConfigs.FirstOrDefault(t => t.name == playerData.MapNodes[0].LocationConfigId);
        generationSettings = selectedLocationConfig.generationSettings;
        foreach (var layerGroup in nodesByLayer)
        {
            List<MapNode> layerNodes = new List<MapNode>();

            foreach (MapNodeData nodeData in layerGroup)
            {
                LocationConfig locationConfig = locationConfigs.FirstOrDefault(config => config.name == nodeData.LocationConfigId) ?? locationConfigs.FirstOrDefault();
                MenuMNG.Instance.SetupLocation(locationConfig._locationName);
                if (locationConfig == null)
                {
                    Debug.LogWarning($"Location config with ID {nodeData.LocationConfigId} not found!");
                    continue;
                }

                TileLogic nodeObject = Instantiate(nodePrefab, mapContainer);
                RectTransform nodeRectTransform = nodeObject.GetComponent<RectTransform>();
                nodeRectTransform.anchoredPosition = nodeData.Position;

                // Find tile configs that match the tile type
                List<NewTileConfig> matchingTileConfigs = new List<NewTileConfig>();
                foreach (var tile in locationConfig.tiles)
                {
                    foreach (var config in tile.tileConfig)
                    {
                        if (config.tileType == nodeData.TileType)
                        {
                            matchingTileConfigs.Add(config);
                        }
                    }
                }

                if (matchingTileConfigs.Count == 0)
                {
                    Debug.LogWarning($"No tile configurations of type {nodeData.TileType} found in location {locationConfig.name}!");
                    continue;
                }

                // Find the specific tile config that was used
                NewTileConfig tileConfig = matchingTileConfigs.FirstOrDefault(t => t.name == nodeData.TileConfigId);
                if (tileConfig == null)
                {
                    // If the specific config is not found, use a random one of the matching type
                    tileConfig = matchingTileConfigs[Random.Range(0, matchingTileConfigs.Count)];
                }

                MapNode mapNode = new MapNode
                {
                    nodeObject = nodeObject,
                    rectTransform = nodeRectTransform,
                    locationConfig = locationConfig,
                    tileType = nodeData.TileType,
                    tileConfig = tileConfig,
                    layerIndex = nodeData.LayerIndex,
                    isVisited = nodeData.IsVisited,
                    isAvailable = nodeData.IsAvailable
                };
                UpdateNodeVisuals(mapNode);
                nodeObject.Initialize(tileConfig.tileSprite, OnNodeClick, mapNode);
                layerNodes.Add(mapNode);
            }

            layers.Add(layerNodes);
        }

        DetermineCurrentAvailableLayer();
        UpdateNodesAvailability();
        CurrentNode = FindLastVisitedNode();
        if (CurrentNode != null)
        {
            if (CurrentNode.tileConfig.tileType == TileType.CampTile)
            {
                CampPanel.Instance.SetupInfo(CurrentNode.tileConfig);
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
    // ========================================
    // == Блок генерации боевого узла карты ==
    // ========================================
    private int lastCampLayerIndex = int.MinValue;

    private List<MapNode> CreateNodesForLayer(int layerIndex, float layerY, int nodesInLayer, ref bool bossPlaced, LocationConfig locationConfig)
    {
        List<MapNode> layerNodes = new List<MapNode>();
        float totalWidth =  mapContainer.rect.width;
        float spacing = totalWidth / (nodesInLayer + 1);
        int totalLayers = generationSettings.numberOfLayers;
        int invertedLayerIndex = totalLayers - 1 - layerIndex;
        float progressFactor = (float)invertedLayerIndex / (totalLayers - 1);

        bool isPreBossLayer = layerIndex == 1;
        bool canSpawnCampThisLayer = (layerIndex <= totalLayers - 2) && (layerIndex - lastCampLayerIndex >= 3);
        int campNodeIndex = isPreBossLayer ? Random.Range(0, nodesInLayer) : -1;
        bool campPlacedThisLayer = false;

        for (int i = 0; i < nodesInLayer; i++)
        {
            TileLogic nodeObject = Instantiate(nodePrefab, mapContainer);
            RectTransform nodeRectTransform = nodeObject.GetComponent<RectTransform>();
            float xPosition = spacing * (i + 1) - totalWidth / 2 + Random.Range(-generationSettings.nodeHorizontalSpread, generationSettings.nodeHorizontalSpread);
            nodeRectTransform.anchoredPosition = new Vector2(xPosition, layerY);

            // Босс на слое 0
            if (!bossPlaced && layerIndex == 0)
            {
                if (TryCreateBossNode(nodeObject, nodeRectTransform, locationConfig, layerIndex, out MapNode bossNode))
                {
                    layerNodes.Add(bossNode);
                    bossPlaced = true;
                    continue;
                }
            }

            TileType selectedTileType;

            // Гарантированный лагерь на предбоссовом слое в случайной позиции
            if (isPreBossLayer && i == campNodeIndex)
            {
                selectedTileType = TileType.CampTile;
                campPlacedThisLayer = true;
            }
            else if (canSpawnCampThisLayer && !campPlacedThisLayer)
            {
                selectedTileType = GetRandomTileType();

                // Принудительно ставим лагерь, если случайно выпал такой тайл
                if (selectedTileType == TileType.CampTile)
                {
                    campPlacedThisLayer = true;
                    lastCampLayerIndex = layerIndex;
                }
            }
            else
            {
                // Убираем шанс лагеря, если уже был или нельзя
                do
                {
                    selectedTileType = GetRandomTileType();
                }
                while (selectedTileType == TileType.CampTile);
            }

            // Боевой тайл
            if (selectedTileType == TileType.BattleTile)
            {
                if (TryCreateBattleNode(nodeObject, nodeRectTransform, locationConfig, layerIndex, progressFactor, out MapNode battleNode))
                {
                    layerNodes.Add(battleNode);
                    continue;
                }
            }

            // Остальные тайлы
            MapNode regularNode = CreateRegularNode(nodeObject, nodeRectTransform, locationConfig, selectedTileType, layerIndex);
            if (regularNode != null)
            {
                layerNodes.Add(regularNode);
            }
        }

        // Обновляем индекс последнего лагеря, если был поставлен гарантированно на предбоссовом слое
        if (isPreBossLayer && campPlacedThisLayer)
            lastCampLayerIndex = layerIndex;

        return layerNodes;
    }

    private bool TryCreateBossNode(TileLogic nodeObject, RectTransform rectTransform, LocationConfig locationConfig, int layerIndex, out MapNode bossNode)
    {
        bossNode = null;
        List<NewTileConfig> bossTileConfigs = locationConfig.tiles
            .Where(t => t.tileType == TileType.BossTile)
            .SelectMany(t => t.tileConfig)
            .ToList();

        if (bossTileConfigs.Count == 0) return false;

        NewTileConfig selectedConfig = bossTileConfigs[Random.Range(0, bossTileConfigs.Count)];

        bossNode = new MapNode
        {
            nodeObject = nodeObject,
            rectTransform = rectTransform,
            locationConfig = locationConfig,
            tileType = TileType.BossTile,
            tileConfig = selectedConfig,
            layerIndex = layerIndex,
            isVisited = false,
            isAvailable = false,
            battleDifficulty = selectedConfig.bossSettings.battleDifficulty
        };

        nodeObject.Initialize(selectedConfig.tileSprite, OnNodeClick, bossNode);
        return true;
    }

    private bool TryCreateBattleNode(TileLogic nodeObject, RectTransform rectTransform, LocationConfig locationConfig, int layerIndex, float progressFactor, out MapNode battleNode)
    {
        battleNode = null;

        var battleTileConfigs = locationConfig.tiles
            .Where(t => t.tileType == TileType.BattleTile)
            .SelectMany(t => t.tileConfig)
            .ToList();

        if (battleTileConfigs.Count == 0)
            return false;

        var (minDifficulty, maxDifficulty) = GetDifficultyRangeForLayer(
            layerIndex,
            generationSettings.numberOfLayers,
            locationConfig.minDifficulty,
            locationConfig.maxDifficulty,
            progressFactor
        );

        var layerBattleConfigs = battleTileConfigs
            .Where(cfg => cfg.battleSettings.battleDifficulty >= minDifficulty &&
                          cfg.battleSettings.battleDifficulty <= maxDifficulty)
            .ToList();

        if (layerBattleConfigs.Count == 0)
        {
            Debug.LogWarning($"Не найдено боевых тайлов для слоя {layerIndex} с диапазоном сложности {minDifficulty}-{maxDifficulty}");
            return false;
        }

        NewTileConfig selectedConfig = SelectBattleConfigWithWeights(layerBattleConfigs, minDifficulty, maxDifficulty);

        battleNode = new MapNode
        {
            nodeObject = nodeObject,
            rectTransform = rectTransform,
            locationConfig = locationConfig,
            tileType = TileType.BattleTile,
            tileConfig = selectedConfig,
            layerIndex = layerIndex,
            isVisited = false,
            isAvailable = false,
            battleDifficulty = selectedConfig.battleSettings.battleDifficulty
        };

        nodeObject.Initialize(selectedConfig.tileSprite, OnNodeClick, battleNode);
        return true;
    }

    private (int min, int max) GetDifficultyRangeForLayer(
        int layerIndex, int totalLayers, int locationMinDifficulty, int locationMaxDifficulty, float progressFactor)
    {
        if (totalLayers <= 1)
            return (locationMinDifficulty, locationMaxDifficulty);

        float normalizedProgress = (float)layerIndex / (totalLayers - 1);
        int difficultyRange = locationMaxDifficulty - locationMinDifficulty;

        int centerDifficulty = Mathf.RoundToInt(
            Mathf.Lerp(locationMaxDifficulty, locationMinDifficulty, normalizedProgress));
        int halfRange = Mathf.Max(1, Mathf.RoundToInt(difficultyRange * 0.1f * progressFactor)); // ±10% в обе стороны

        int minDifficulty = centerDifficulty - halfRange;
        int maxDifficulty = centerDifficulty + halfRange;

        // Клэмпим значения в допустимых пределах
        minDifficulty = Mathf.Clamp(minDifficulty, locationMinDifficulty, locationMaxDifficulty);
        maxDifficulty = Mathf.Clamp(maxDifficulty, locationMinDifficulty, locationMaxDifficulty);

        // Подстраховка: min ≤ max
        if (minDifficulty > maxDifficulty)
            minDifficulty = maxDifficulty;

        return (minDifficulty, maxDifficulty);
    }


    private NewTileConfig SelectBattleConfigWithWeights(List<NewTileConfig> configs, int minDifficulty, int maxDifficulty)
    {
        if (configs.Count == 1)
            return configs[0];

        var groupedByDifficulty = configs
            .GroupBy(c => c.battleSettings.battleDifficulty)
            .ToDictionary(g => g.Key, g => g.ToList());

        Dictionary<int, float> weights = new Dictionary<int, float>();
        float totalWeight = 0f;

        foreach (var kvp in groupedByDifficulty)
        {
            int difficulty = kvp.Key;
            float weight = 1f;

            if (difficulty == minDifficulty || difficulty == maxDifficulty)
                weight *= 1.2f;

            weights[difficulty] = weight;
            totalWeight += weight;
        }

        float randomValue = Random.Range(0, totalWeight);
        float currentSum = 0f;

        foreach (var kvp in weights.OrderBy(k => k.Key))
        {
            currentSum += kvp.Value;
            if (randomValue <= currentSum)
            {
                var configsForDifficulty = groupedByDifficulty[kvp.Key];
                return configsForDifficulty[Random.Range(0, configsForDifficulty.Count)];
            }
        }

        return configs[Random.Range(0, configs.Count)];
    }




    private MapNode CreateRegularNode(TileLogic nodeObject, RectTransform rectTransform, LocationConfig locationConfig, TileType tileType, int layerIndex)
    {
        List<NewTileConfig> matchingConfigs = locationConfig.tiles
            .Where(t => t.tileType == tileType)
            .SelectMany(t => t.tileConfig)
            .ToList();

        if (matchingConfigs.Count == 0)
        {
            matchingConfigs = locationConfig.tiles
                .SelectMany(t => t.tileConfig)
                .ToList();

            if (matchingConfigs.Count == 0)
                return null;

            tileType = matchingConfigs[0].tileType;
        }

        NewTileConfig selectedConfig = matchingConfigs[Random.Range(0, matchingConfigs.Count)];

        int difficulty = tileType switch
        {
            TileType.BattleTile => selectedConfig.battleSettings.battleDifficulty,
            TileType.BossTile => selectedConfig.bossSettings.battleDifficulty,
            _ => 0
        };

        MapNode node = new MapNode
        {
            nodeObject = nodeObject,
            rectTransform = rectTransform,
            locationConfig = locationConfig,
            tileType = tileType,
            tileConfig = selectedConfig,
            layerIndex = layerIndex,
            isVisited = false,
            isAvailable = false,
            battleDifficulty = difficulty
        };

        nodeObject.Initialize(selectedConfig.tileSprite, OnNodeClick, node);
        return node;
    }

    // ========================================
    // == Блок генерации боевого узла карты ==
    // ========================================

    private void HandleTileClicked(MapNode node)
    {
        if (!node.isAvailable) return;
        currentNode = node;
        GameDataMNG.Instance.HandleTileClick(currentNode);
        UpdateMap();
    }

    public void UpdateMap()
    {
        currentNode.isVisited = true;
        currentNode.isAvailable = false;

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
        TileUIController.Instance.ChangeUIOnTileClick();
        SaveMapToPlayerData();
    }

    public void OnNodeClick(MapNode node)
    {
        OnTileClicked?.Invoke(node);
    }

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

    private void ClearExistingNodes()
    {
        foreach (Transform child in mapContainer)
        {
            Destroy(child.gameObject);
        }
    }
}