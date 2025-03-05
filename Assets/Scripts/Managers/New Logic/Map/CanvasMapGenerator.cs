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
        public int maxNodes = 20;
        public int minNodesPerLayer = 3;
        public int maxNodesPerLayer = 7;
        public float layerHeight = 150f;
        public List<LocationConfig> locationConfigs;
        [Header("Randomization")]
        public float nodeHorizontalSpread = 50f;
        public List<TileSpawnProbability> tileSpawnProbabilities; // Вероятности появления тайлов
    }

    [System.Serializable]
    public class TileSpawnProbability
    {
        public TileType tileType;
        public float probability; // Вероятность от 0 до 1
    }

    [System.Serializable]
    public class MapNode
    {
        public GameObject nodeObject;
        public RectTransform rectTransform;
        public LocationConfig newTileConfig; // Ссылка на конфиг тайла
        public Image nodeImage; // Компонент Image для отображения спрайта
    }

    public MapGenerationSettings generationSettings;
    private List<List<MapNode>> layers = new List<List<MapNode>>();

    // Событие для клика по тайлу
    public UnityEvent<TileType> OnTileClicked;

    public void GenerateMap()
    {
        ClearExistingNodes();
        layers.Clear();
        float currentLayerY = generationSettings.mapContainer.rect.height / 2;
        int totalNodesCreated = 0;
        bool bossPlaced = false; // Флаг для отслеживания того, что босса уже поставили

        for (int layerIndex = 0; totalNodesCreated < generationSettings.maxNodes; layerIndex++)
        {
            int nodesInLayer = layerIndex == 0 ? 1 : DetermineNodesForLayer(totalNodesCreated); // 1 узел на первом слое
            List<MapNode> layerNodes = CreateNodesForLayer(currentLayerY, nodesInLayer, ref bossPlaced);
            layers.Add(layerNodes);
            totalNodesCreated += nodesInLayer;
            currentLayerY -= generationSettings.layerHeight;
           // if (currentLayerY < -generationSettings.mapContainer.rect.height / 2) break;
        }
    }

    private List<MapNode> CreateNodesForLayer(float layerY, int nodesInLayer, ref bool bossPlaced)
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

            // Если босс еще не был установлен, установим его на первый доступный тайл
            if (!bossPlaced)
            {
                LocationConfig selectedLocationConfig = generationSettings.locationConfigs.FirstOrDefault(config =>
                    config.tiles.Any(tile => tile.tileConfig.tileType == TileType.BossTile));

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
                            newTileConfig = selectedLocationConfig, // Сохраняем конфиг локации
                            nodeImage = nodeObject.GetComponent<Image>(),
                        };

                        newNode.nodeImage.sprite = bossTile.tileConfig.tileSprite;

                        Button nodeButton = nodeObject.GetComponent<Button>();
                        if (nodeButton != null)
                        {
                            nodeButton.onClick.AddListener(() => OnTileClick(newNode.newTileConfig.tiles[0].tileConfig.tileType)); // Используем конфиг для вызова события
                        }

                        layerNodes.Add(newNode);
                        bossPlaced = true;
                        continue; // Переходим к следующей итерации
                    }
                }
            }

            TileType selectedTileType = GetRandomTileType();
            LocationConfig selectedTileConfig = generationSettings.locationConfigs[Random.Range(0,generationSettings.locationConfigs.Count())];
            LocationConfig.LocationTile generatedTile = selectedTileConfig.tiles
                .FirstOrDefault(tile => tile.tileConfig.tileType == selectedTileType);

            MapNode newTileNode = new MapNode
            {
                nodeObject = nodeObject,
                rectTransform = nodeRectTransform,
                newTileConfig = selectedTileConfig, // Сохраняем конфиг локации
                nodeImage = nodeObject.GetComponent<Image>(),
            };

            newTileNode.nodeImage.sprite = generatedTile.tileConfig.tileSprite;

            Button nodeButtonNormal = nodeObject.GetComponent<Button>();
            if (nodeButtonNormal != null)
            {
                nodeButtonNormal.onClick.AddListener(() => OnTileClick(newTileNode.newTileConfig.tiles[0].tileConfig.tileType)); // Используем конфиг для вызова события
            }

            layerNodes.Add(newTileNode);
        }

        return layerNodes;
    }

    private TileType GetRandomTileType()
    {
        // Суммируем все вероятности
        float totalProbability = generationSettings.tileSpawnProbabilities.Sum(t => t.probability);

        // Если сумма вероятностей равна 0, возвращаем дефолтный тайл (например, BattleTile)
        if (totalProbability <= 0f)
        {
            Debug.LogWarning("All tile spawn probabilities are 0. Returning default BattleTile.");
            return TileType.BattleTile; // Возвращаем тип по умолчанию
        }

        // Нормализуем вероятности, если сумма меньше 1
        if (totalProbability < 1f)
        {
            float normalizationFactor = 1f / totalProbability;
            foreach (var item in generationSettings.tileSpawnProbabilities)
            {
                item.probability *= normalizationFactor;
            }
            totalProbability = 1f; // После нормализации сумма должна быть 1
        }

        // Получаем случайное значение, которое будет распределяться по вероятностям
        float randomValue = Random.Range(0f, totalProbability);

        // Итерируем по всем вероятностям и выбираем нужный тайл
        foreach (TileSpawnProbability tileProb in generationSettings.tileSpawnProbabilities)
        {
            randomValue -= tileProb.probability;
            if (randomValue <= 0f)
            {
                return tileProb.tileType;
            }
        }

        // Если по каким-то причинам вероятность не сработала, возвращаем дефолтный тайл
        return TileType.BattleTile; // Можно вернуть другой дефолтный тип
    }

    private void ClearExistingNodes()
    {
        foreach (Transform child in generationSettings.mapContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private int DetermineNodesForLayer(int currentNodeCount)
    {
        int remainingNodes = generationSettings.maxNodes - currentNodeCount;
        return Random.Range(generationSettings.minNodesPerLayer, Mathf.Min(generationSettings.maxNodesPerLayer, remainingNodes) + 1);
    }

    private void OnTileClick(TileType tileType)
    {
        OnTileClicked?.Invoke(tileType);
    }

    [ContextMenu("Generate Map")]
    public void EditorGenerateMap()
    {
        GenerateMap();
    }
}
