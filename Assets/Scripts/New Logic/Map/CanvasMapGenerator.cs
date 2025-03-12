using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

public class CanvasMapGenerator : MonoBehaviour
{
    [System.Serializable]
    public class MapGenerationSettings
    {
        [Header("Map Layout")]
        public RectTransform mapContainer;
        public GameObject nodePrefab;
        public int numberOfLayers = 5; // Changed from maxNodes to numberOfLayers
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
        public float probability; // Probability from 0 to 1
    }

    [System.Serializable]
    public class MapNode
    {
        public GameObject nodeObject;
        public RectTransform rectTransform;
        public LocationConfig locationConfig;
        public TileType tileType;
        public Image nodeImage;
        public int layerIndex;
        public bool isVisited;
        public bool isAvailable;
        public Button nodeButton;
    }

    public MapGenerationSettings generationSettings;
    private List<List<MapNode>> layers = new List<List<MapNode>>();
    private int currentAvailableLayer;

    // Events to notify about clicks and map state
    public UnityEvent<TileType, MapNode> OnTileClicked;
    public UnityEvent<int> OnLayerCompleted;

    public void GenerateMap()
    {
        ClearExistingNodes();
        layers.Clear();
        float currentLayerY = generationSettings.mapContainer.rect.height / 2;
        bool bossPlaced = false;

        // Generate specified number of layers instead of checking total nodes
        for (int layerIndex = 0; layerIndex < generationSettings.numberOfLayers; layerIndex++)
        {
            int nodesInLayer = layerIndex == 0 ? 1 : Random.Range(
                generationSettings.minNodesPerLayer,
                generationSettings.maxNodesPerLayer + 1);

            List<MapNode> layerNodes = CreateNodesForLayer(layerIndex, currentLayerY, nodesInLayer, ref bossPlaced);
            layers.Add(layerNodes);
            currentLayerY -= generationSettings.layerHeight;
        }

        // Set initial available layer (last layer)
        SetInitialAvailableLayer();
    }

    // Set initial available layer (last layer)
    private void SetInitialAvailableLayer()
    {
        currentAvailableLayer = layers.Count - 1;
        UpdateNodesAvailability();
    }

    // Update node availability based on current layer
    private void UpdateNodesAvailability()
    {
        // Block all nodes
        foreach (var layer in layers)
        {
            foreach (var node in layer)
            {
                node.isAvailable = false;
                UpdateNodeVisuals(node);
            }
        }

        // If all layers have been visited, exit
        if (currentAvailableLayer < 0)
            return;

        // Make available only nodes in current layer that haven't been visited
        foreach (var node in layers[currentAvailableLayer])
        {
            if (!node.isVisited)
            {
                node.isAvailable = true;
                UpdateNodeVisuals(node);
            }
        }
    }

    // Update visual representation of node based on its state
    private void UpdateNodeVisuals(MapNode node)
    {
        if (node.isVisited)
        {
            // Visited nodes
            node.nodeImage.color = generationSettings.visitedNodeColor;
            node.nodeButton.interactable = false;
        }
        else if (node.isAvailable)
        {
            // Available nodes
            node.nodeImage.color = generationSettings.availableNodeColor;
            node.nodeButton.interactable = true;
        }
        else
        {
            // Unavailable nodes
            node.nodeImage.color = generationSettings.unavailableNodeColor;
            node.nodeButton.interactable = false;
        }
    }

    // Load map from PlayerData
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

        // Group nodes by layers
        var nodesByLayer = playerData.MapNodes.GroupBy(n => n.LayerIndex).OrderBy(g => g.Key);

        foreach (var layerGroup in nodesByLayer)
        {
            List<MapNode> layerNodes = new List<MapNode>();

            foreach (MapNodeData nodeData in layerGroup)
            {
                // Find corresponding location config by ID
                LocationConfig locationConfig = generationSettings.locationConfigs.FirstOrDefault(
                    config => config.name == nodeData.LocationConfigId);

                if (locationConfig == null)
                {
                    Debug.LogWarning($"Location config with ID {nodeData.LocationConfigId} not found!");
                    locationConfig = generationSettings.locationConfigs.FirstOrDefault();
                }

                // Create game object for node
                GameObject nodeObject = Instantiate(generationSettings.nodePrefab, generationSettings.mapContainer);
                RectTransform nodeRectTransform = nodeObject.GetComponent<RectTransform>();
                nodeRectTransform.anchoredPosition = nodeData.Position;

                // Find tile with specified type in location config
                LocationConfig.LocationTile tileConfig = locationConfig.tiles.FirstOrDefault(
                    tile => tile.tileConfig.tileType == nodeData.TileType);

                if (tileConfig == null)
                {
                    Debug.LogWarning($"Tile type {nodeData.TileType} not found in location config {locationConfig.name}!");
                    tileConfig = locationConfig.tiles.FirstOrDefault();
                }

                // Create MapNode and configure it
                Button nodeButton = nodeObject.GetComponent<Button>();

                MapNode mapNode = new MapNode
                {
                    nodeObject = nodeObject,
                    rectTransform = nodeRectTransform,
                    locationConfig = locationConfig,
                    tileType = nodeData.TileType,
                    nodeImage = nodeObject.GetComponent<Image>(),
                    layerIndex = nodeData.LayerIndex,
                    isVisited = nodeData.IsVisited,
                    isAvailable = nodeData.IsAvailable,
                    nodeButton = nodeButton
                };

                // Set sprite
                if (tileConfig != null)
                {
                    mapNode.nodeImage.sprite = tileConfig.tileConfig.tileSprite;
                }

                // Set click handler
                if (nodeButton != null)
                {
                    // Use local variable to capture mapNode
                    MapNode capturedNode = mapNode;
                    nodeButton.onClick.AddListener(() => OnNodeClick(capturedNode));
                }

                layerNodes.Add(mapNode);
            }

            layers.Add(layerNodes);
        }

        // Determine current available layer based on saved data
        DetermineCurrentAvailableLayer();
        UpdateNodesAvailability();
    }

    // Determine current available layer based on visited nodes
    private void DetermineCurrentAvailableLayer()
    {
        // Iterate through layers starting from the last
        for (int i = layers.Count - 1; i >= 0; i--)
        {
            // If there's at least one visited node in the layer, then the next layer is available
            if (layers[i].Any(node => node.isVisited))
            {
                currentAvailableLayer = i - 1;
                return;
            }
        }

        // If no node has been visited, the last layer is available
        currentAvailableLayer = layers.Count - 1;
    }

    // Save current map to PlayerData
    public void SaveMapToPlayerData()
    {
        List<MapNodeData> mapNodesData = new List<MapNodeData>();

        for (int layerIndex = 0; layerIndex < layers.Count; layerIndex++)
        {
            foreach (MapNode node in layers[layerIndex])
            {
                MapNodeData nodeData = new MapNodeData(
                    node.rectTransform.anchoredPosition,
                    node.tileType,
                    layerIndex,
                    node.locationConfig.name);

                nodeData.IsVisited = node.isVisited;
                nodeData.IsAvailable = node.isAvailable;
                mapNodesData.Add(nodeData);
            }
        }

        GameDataMNG.Instance.PlayerData.MapNodes = mapNodesData;
    }

    private List<MapNode> CreateNodesForLayer(int layerIndex, float layerY, int nodesInLayer, ref bool bossPlaced)
    {
        List<MapNode> layerNodes = new List<MapNode>();
        float totalWidth = generationSettings.mapContainer.rect.width;
        float spacing = totalWidth / (nodesInLayer + 1);

        for (int i = 0; i < nodesInLayer; i++)
        {
            GameObject nodeObject = Instantiate(generationSettings.nodePrefab, generationSettings.mapContainer);
            RectTransform nodeRectTransform = nodeObject.GetComponent<RectTransform>();
            float xPosition = spacing * (i + 1) - totalWidth / 2 + Random.Range(-generationSettings.nodeHorizontalSpread, generationSettings.nodeHorizontalSpread);
            nodeRectTransform.anchoredPosition = new Vector2(xPosition, layerY);
            Button nodeButton = nodeObject.GetComponent<Button>();
            LocationConfig selectedLocationConfig = null;
            // If boss hasn't been placed yet and this is the first layer, place it there
            if (!bossPlaced && layerIndex == 0)
            {
                selectedLocationConfig = generationSettings.locationConfigs[Random.Range(0, generationSettings.locationConfigs.Count)];

                if (selectedLocationConfig != null)
                {
                    LocationConfig.LocationTile bossTile = selectedLocationConfig.tiles
                        .FirstOrDefault(tile => tile.tileConfig.tileType == TileType.BossTile);

                    if (bossTile != null)
                    {
                        MapNode newNode = new MapNode
                        {
                            nodeObject = nodeObject,
                            rectTransform = nodeRectTransform,
                            locationConfig = selectedLocationConfig,
                            tileType = TileType.BossTile,
                            nodeImage = nodeObject.GetComponent<Image>(),
                            layerIndex = layerIndex,
                            isVisited = false,
                            isAvailable = false,
                            nodeButton = nodeButton
                        };

                        newNode.nodeImage.sprite = bossTile.tileConfig.tileSprite;

                        if (nodeButton != null)
                        {
                            MapNode capturedNode = newNode;
                            nodeButton.onClick.AddListener(() => OnNodeClick(capturedNode));
                        }

                        layerNodes.Add(newNode);
                        bossPlaced = true;
                        continue; // Move to next iteration
                    }
                }
            }

            TileType selectedTileType = GetRandomTileType();
            selectedLocationConfig = generationSettings.locationConfigs[Random.Range(0, generationSettings.locationConfigs.Count)];
            LocationConfig.LocationTile generatedTile = selectedLocationConfig.tiles
                .FirstOrDefault(tile => tile.tileConfig.tileType == selectedTileType);

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
                nodeImage = nodeObject.GetComponent<Image>(),
                layerIndex = layerIndex,
                isVisited = false,
                isAvailable = false,
                nodeButton = nodeButton
            };

            newTileNode.nodeImage.sprite = generatedTile.tileConfig.tileSprite;

            if (nodeButton != null)
            {
                MapNode capturedNode = newTileNode;
                nodeButton.onClick.AddListener(() => OnNodeClick(capturedNode));
            }

            layerNodes.Add(newTileNode);
        }

        return layerNodes;
    }

    // Node click handler
    private void OnNodeClick(MapNode node)
    {
        if (!node.isAvailable)
            return;

        // Mark node as visited
        node.isVisited = true;
        node.isAvailable = false;

        // Mark all nodes in current layer as unavailable
        foreach (var layerNode in layers[currentAvailableLayer])
        {
            layerNode.isAvailable = false;
        }

        // Check if all nodes in current layer have been visited
        bool allNodesInLayerVisited = layers[currentAvailableLayer].All(n => n.isVisited);

        // If all nodes in current layer have been visited, move to next layer
        if (!allNodesInLayerVisited)
        {
            // If not, just block all nodes in current layer
            UpdateNodesAvailability();
        }

        // Move to next level (up, i.e., decrease index)
        currentAvailableLayer--;

        // If we've reached the first layer or above, the map is completed
        if (currentAvailableLayer < 0)
        {
            Debug.Log("Map fully completed!");
            // Can trigger map completion event
        }
        else
        {
            // Otherwise make next layer available
            UpdateNodesAvailability();

            // Trigger layer completion event
            OnLayerCompleted?.Invoke(currentAvailableLayer + 1);
        }

        // Trigger tile click event
        OnTileClicked?.Invoke(node.tileType, node);
    }

    private TileType GetRandomTileType()
    {
        // Sum all probabilities
        float totalProbability = generationSettings.tileSpawnProbabilities.Sum(t => t.probability);

        // If sum of probabilities is 0, return default tile (e.g., BattleTile)
        if (totalProbability <= 0f)
        {
            Debug.LogWarning("All tile spawn probabilities are 0. Returning default BattleTile.");
            return TileType.BattleTile; // Return default type
        }

        // Normalize probabilities if sum is less than 1
        if (totalProbability < 1f)
        {
            float normalizationFactor = 1f / totalProbability;
            foreach (var item in generationSettings.tileSpawnProbabilities)
            {
                item.probability *= normalizationFactor;
            }
            totalProbability = 1f; // After normalization, sum should be 1
        }

        // Get random value to be distributed by probabilities
        float randomValue = Random.Range(0f, totalProbability);

        // Iterate through all probabilities and choose required tile
        foreach (TileSpawnProbability tileProb in generationSettings.tileSpawnProbabilities)
        {
            randomValue -= tileProb.probability;
            if (randomValue <= 0f)
            {
                return tileProb.tileType;
            }
        }

        // If for some reason probability didn't work, return default tile
        return TileType.BattleTile;
    }

    private void ClearExistingNodes()
    {
        foreach (Transform child in generationSettings.mapContainer)
        {
            Destroy(child.gameObject);
        }
    }

    // Method to check node availability (for external calls)
    public bool IsNodeAvailable(Vector2 position)
    {
        foreach (var layer in layers)
        {
            foreach (var node in layer)
            {
                if (Vector2.Distance(node.rectTransform.anchoredPosition, position) < 10f)
                {
                    return node.isAvailable;
                }
            }
        }
        return false;
    }

    // Method for manual activation of next layer (may be useful for testing)
    [ContextMenu("Activate Next Layer Up")]
    public void ActivateNextLayerUp()
    {
        if (currentAvailableLayer > 0)
        {
            currentAvailableLayer--;
            UpdateNodesAvailability();
        }
    }

    [ContextMenu("Generate Map")]
    public void EditorGenerateMap()
    {
        GenerateMap();
    }
}