using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class CanvasMapGenerator : MonoBehaviour
{
    [System.Serializable]
    public class MapGenerationSettings
    {
        [Header("Map Layout")]
        public RectTransform mapContainer;
        public GameObject nodePrefab;
        public int maxNodes = 20;
        public int minNodesPerLayer = 3;
        public int maxNodesPerLayer = 7;
        public float layerHeight = 150f;

        [Header("Randomization")]
        public float nodeHorizontalSpread = 50f;
    }

    [System.Serializable]
    public class MapNode
    {
        public GameObject nodeObject;
        public RectTransform rectTransform;
        public Image nodeImage;
        public NewTileConfig tileConfig;
        public Vector2Int gridPosition;
        public List<MapNode> connectedNodes = new List<MapNode>();
    }

    public MapGenerationSettings generationSettings;
    public LocationConfig locationConfig;

    private List<MapNode> generatedNodes = new List<MapNode>();

    public void GenerateMap()
    {
        ClearExistingNodes();

        // Start from the bottom of the container
        float currentLayerY = -generationSettings.mapContainer.rect.height;
        int totalNodesCreated = 0;

        while (totalNodesCreated < generationSettings.maxNodes)
        {
            int nodesInCurrentLayer = DetermineNodesForLayer(totalNodesCreated);
            CreateNodesForLayer(currentLayerY, nodesInCurrentLayer);

            totalNodesCreated += nodesInCurrentLayer;
            currentLayerY += generationSettings.layerHeight;

            // Stop if we've reached the top of the container
            if (currentLayerY > 0) break;
        }

        ConnectNodes();
    }

    private void ClearExistingNodes()
    {
        foreach (var node in generatedNodes)
        {
            if (node.nodeObject != null)
            {
                Destroy(node.nodeObject);
            }
        }
        generatedNodes.Clear();
    }

    private int DetermineNodesForLayer(int currentNodeCount)
    {
        int remainingNodes = generationSettings.maxNodes - currentNodeCount;
        return Random.Range(
            generationSettings.minNodesPerLayer,
            Mathf.Min(generationSettings.maxNodesPerLayer, remainingNodes) + 1
        );
    }

    private void CreateNodesForLayer(float layerY, int nodesInLayer)
    {
        Vector2 containerSize = generationSettings.mapContainer.rect.size;
        float totalWidth = containerSize.x;

        // Evenly distribute nodes across the container's width
        float spacing = totalWidth / (nodesInLayer + 1);

        for (int i = 0; i < nodesInLayer; i++)
        {
            MapNode newNode = CreateSingleNode(layerY, spacing, i, nodesInLayer);
            if (newNode != null)
            {
                generatedNodes.Add(newNode);
            }
        }
    }

    private MapNode CreateSingleNode(float layerY, float spacing, int index, int totalNodesInLayer)
    {
        NewTileConfig selectedTileConfig = SelectNodeTileConfig();
        GameObject nodeObject = Instantiate(generationSettings.nodePrefab, generationSettings.mapContainer);

        RectTransform nodeRectTransform = nodeObject.GetComponent<RectTransform>();
        Image nodeImage = nodeObject.GetComponent<Image>();

        // Calculate base position
        float baseX = spacing * (index + 1) - (generationSettings.mapContainer.rect.width / 2);

        // Add controlled randomness to each node
        float randomOffsetX = Random.Range(-generationSettings.nodeHorizontalSpread, generationSettings.nodeHorizontalSpread);
        float xPosition = baseX + randomOffsetX;

        // Position node
        nodeRectTransform.anchoredPosition = new Vector2(xPosition, layerY);

        // Configure node image
        if (nodeImage != null && selectedTileConfig.sprite != null)
        {
            nodeImage.sprite = selectedTileConfig.sprite;
        }

        // Create map node
        MapNode mapNode = new MapNode
        {
            nodeObject = nodeObject,
            rectTransform = nodeRectTransform,
            nodeImage = nodeImage,
            tileConfig = selectedTileConfig,
            gridPosition = new Vector2Int(index, (int)layerY)
        };

        return mapNode;
    }

    private NewTileConfig SelectNodeTileConfig()
    {
        if (locationConfig.tiles == null || locationConfig.tiles.Count == 0)
        {
            Debug.LogError("No tile configurations available!");
            return null;
        }

        return locationConfig.tiles[Random.Range(0, locationConfig.tiles.Count)].tileConfig;
    }

    private void ConnectNodes()
    {
        var nodesByLayer = generatedNodes
            .GroupBy(n => n.gridPosition.y)
            .OrderBy(g => g.Key)
            .ToList();

        for (int i = 0; i < nodesByLayer.Count - 1; i++)
        {
            var currentLayer = nodesByLayer[i];
            var nextLayer = nodesByLayer[i + 1];

            ConnectLayerNodes(currentLayer, nextLayer);
        }
    }

    private void ConnectLayerNodes(
        IGrouping<int, MapNode> currentLayer,
        IGrouping<int, MapNode> nextLayer)
    {
        foreach (var currentNode in currentLayer)
        {
            var possibleConnections = FindPossibleConnections(currentNode, nextLayer);
            ConnectToNearestNodes(currentNode, possibleConnections, 2);
        }
    }

    private List<MapNode> FindPossibleConnections(
        MapNode currentNode,
        IGrouping<int, MapNode> nextLayer)
    {
        return nextLayer
            .OrderBy(nextNode => Mathf.Abs(currentNode.rectTransform.anchoredPosition.x - nextNode.rectTransform.anchoredPosition.x))
            .ToList();
    }

    private void ConnectToNearestNodes(MapNode currentNode, List<MapNode> possibleConnections, int maxConnections = 1)
    {
        if (possibleConnections.Count == 0) return;

        int connectionsToMake = Mathf.Min(maxConnections, possibleConnections.Count);

        for (int i = 0; i < connectionsToMake; i++)
        {
            var connectionNode = possibleConnections[i];

            if (!currentNode.connectedNodes.Contains(connectionNode))
            {
                currentNode.connectedNodes.Add(connectionNode);
                connectionNode.connectedNodes.Add(currentNode);

                DrawConnectionLine(currentNode, connectionNode);
            }
        }
    }

    private void DrawConnectionLine(MapNode fromNode, MapNode toNode)
    {
        // Create a new GameObject for drawing the line
        GameObject lineObject = new GameObject("ConnectionLine");
        lineObject.transform.SetParent(generationSettings.mapContainer, false);

        // Add LineRenderer component
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        // Configure line renderer
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.startWidth = 5f;
        lineRenderer.endWidth = 5f;
        lineRenderer.positionCount = 2;

        // Set line positions
        lineRenderer.SetPosition(0, fromNode.rectTransform.position);
        lineRenderer.SetPosition(1, toNode.rectTransform.position);

        // Ensure the line is behind the nodes
        lineRenderer.sortingOrder = -1;
    }

    [ContextMenu("Generate Map")]
    public void EditorGenerateMap()
    {
        GenerateMap();
    }
}