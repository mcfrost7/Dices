using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LocationMapGenerator : MonoBehaviour
{
    [System.Serializable]
    public class MapTileNode
    {
        public LocationConfig.LocationTile tile;
        public GameObject tileObject;
        public List<MapTileNode> connections = new List<MapTileNode>();
    }

    [SerializeField] private List<LocationConfig> locationConfigs;
    [SerializeField] private float nodeSpacing = 200f;
    [SerializeField] private float minTileDistance = 120f; // Минимальное расстояние между тайлами
    [SerializeField] private int maxTileCount = 20;
    [SerializeField] private RectTransform tileContainer;
    [SerializeField] private GameObject tilePrefab;

    private List<MapTileNode> currentLocationNodes = new List<MapTileNode>();

    public List<LocationConfig> LocationConfigs { get => locationConfigs; set => locationConfigs = value; }
    public float NodeSpacing { get => nodeSpacing; set => nodeSpacing = value; }
    public RectTransform TileContainer { get => tileContainer; set => tileContainer = value; }
    public GameObject TilePrefab { get => tilePrefab; set => tilePrefab = value; }

    public void GenerateRandomLocationMap(LocationConfig locationConfig, int gridWidth, int gridHeight)
    {
        ClearPreviousMap();

        if (locationConfig.tiles == null || locationConfig.tiles.Count == 0)
        {
            Debug.LogError("No tiles available in location configuration");
            return;
        }

        var randomPositions = GenerateRandomGridPositions(gridWidth, gridHeight);
        var randomTiles = locationConfig.tiles;

        int tilesCreated = 0;
        var occupiedPositions = new List<Vector2>();

        foreach (var randomPos in randomPositions)
        {
            if (tilesCreated >= maxTileCount) break;

            var position = new Vector3(
                randomPos.x * nodeSpacing,
                -randomPos.y * nodeSpacing,
                0
            );

            if (!IsPositionTooClose(position, occupiedPositions))
            {
                var randomTile = randomTiles[Random.Range(0, randomTiles.Count)];
                CreateTileNode(new LocationConfig.LocationTile
                {
                    gridPosition = randomPos,
                    tileConfig = randomTile.tileConfig
                });

                occupiedPositions.Add(position);
                tilesCreated++;
            }
        }

        ConnectAdjacentNodes();
    }

    private bool IsPositionTooClose(Vector3 position, List<Vector2> occupiedPositions)
    {
        foreach (var occupiedPos in occupiedPositions)
        {
            if (Vector2.Distance(position, occupiedPos) < minTileDistance)
            {
                return true;
            }
        }
        return false;
    }

    private List<Vector2Int> GenerateRandomGridPositions(int gridWidth, int gridHeight)
    {
        var positions = new List<Vector2Int>();
        var allPositions = new List<Vector2Int>();

        for (int x = 0; x < gridWidth; x++)
            for (int y = 0; y < gridHeight; y++)
                allPositions.Add(new Vector2Int(x, y));

        while (positions.Count < gridWidth * gridHeight)
        {
            int index = Random.Range(0, allPositions.Count);
            positions.Add(allPositions[index]);
            allPositions.RemoveAt(index);
        }

        return positions;
    }

    private void CreateTileNode(LocationConfig.LocationTile locationTile)
    {
        var position = new Vector3(
            locationTile.gridPosition.x * NodeSpacing,
            -locationTile.gridPosition.y * NodeSpacing,
            0
        );

        var tileObject = Instantiate(TilePrefab, position, Quaternion.identity, TileContainer);
        var imageComponent = tileObject.GetComponent<Image>();
        var rectTransform = tileObject.GetComponent<RectTransform>();

        if (imageComponent && locationTile.tileConfig.sprite)
        {
            imageComponent.sprite = locationTile.tileConfig.sprite;
            imageComponent.enabled = true;
            imageComponent.type = Image.Type.Simple;
            imageComponent.preserveAspect = true;
        }

        if (rectTransform)
        {
            rectTransform.anchoredPosition = new Vector2(position.x, position.y);
            rectTransform.sizeDelta = new Vector2(100, 100);
        }

        currentLocationNodes.Add(new MapTileNode
        {
            tile = locationTile,
            tileObject = tileObject
        });
    }

    private void ConnectAdjacentNodes()
    {
        for (int i = 0; i < currentLocationNodes.Count; i++)
        {
            for (int j = i + 1; j < currentLocationNodes.Count; j++)
            {
                var nodeA = currentLocationNodes[i];
                var nodeB = currentLocationNodes[j];

                if (AreNodesAdjacent(nodeA, nodeB))
                {
                    ConnectNodes(nodeA, nodeB);
                }
            }
        }
    }

    private bool AreNodesAdjacent(MapTileNode nodeA, MapTileNode nodeB)
    {
        var posA = nodeA.tile.gridPosition;
        var posB = nodeB.tile.gridPosition;
        return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y) == 1;
    }

    private void ConnectNodes(MapTileNode nodeA, MapTileNode nodeB)
    {
        nodeA.connections.Add(nodeB);
        nodeB.connections.Add(nodeA);

        var connectionLine = new GameObject("Connection");
        connectionLine.transform.SetParent(TileContainer);

        var lineImage = connectionLine.AddComponent<Image>();
        var lineRectTransform = connectionLine.GetComponent<RectTransform>();

        var startPos = nodeA.tileObject.GetComponent<RectTransform>().anchoredPosition;
        var endPos = nodeB.tileObject.GetComponent<RectTransform>().anchoredPosition;

        var angle = Mathf.Atan2(endPos.y - startPos.y, endPos.x - startPos.x) * Mathf.Rad2Deg;
        var distance = Vector2.Distance(startPos, endPos);

        lineImage.color = Color.white;
        lineRectTransform.anchoredPosition = (startPos + endPos) / 2;
        lineRectTransform.sizeDelta = new Vector2(distance, 50f);
        lineRectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void ClearPreviousMap()
    {
        foreach (Transform child in TileContainer)
        {
            Destroy(child.gameObject);
        }
        currentLocationNodes.Clear();
    }

    public void GenerateRandomMap(int width = 5, int height = 5)
    {
        if (LocationConfigs.Count > 0)
        {
            GenerateRandomLocationMap(LocationConfigs[0], width, height);
        }
    }

    [ContextMenu("Generate Random Location Map")]
    private void GenerateRandomMapFromMenu() => GenerateRandomMap();
}