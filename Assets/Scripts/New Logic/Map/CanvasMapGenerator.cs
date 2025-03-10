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
        [Header("Appearance")]
        public Color availableNodeColor = Color.white;
        public Color visitedNodeColor = new Color(0.7f, 0.7f, 0.7f);
        public Color unavailableNodeColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
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
        public LocationConfig locationConfig; // Ссылка на конфиг локации
        public TileType tileType; // Тип тайла
        public Image nodeImage; // Компонент Image для отображения спрайта
        public int layerIndex; // Индекс слоя
        public bool isVisited; // Посещен ли нод
        public bool isAvailable; // Доступен ли нод для посещения
        public Button nodeButton; // Кнопка нода
    }

    public MapGenerationSettings generationSettings;
    private List<List<MapNode>> layers = new List<List<MapNode>>();
    private int currentAvailableLayer; // Текущий доступный слой для посещения

    // События для оповещения о кликах и состоянии карты
    public UnityEvent<TileType, MapNode> OnTileClicked;
    public UnityEvent<int> OnLayerCompleted; // Вызывается, когда слой завершен

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
            List<MapNode> layerNodes = CreateNodesForLayer(layerIndex, currentLayerY, nodesInLayer, ref bossPlaced);
            layers.Add(layerNodes);
            totalNodesCreated += nodesInLayer;
            currentLayerY -= generationSettings.layerHeight;
        }

        // Установка начального доступного слоя (последний слой)
        SetInitialAvailableLayer();
    }

    // Задаем начальный доступный слой (последний)
    private void SetInitialAvailableLayer()
    {
        currentAvailableLayer = layers.Count - 1;
        UpdateNodesAvailability();
    }

    // Обновление доступности нодов в зависимости от текущего слоя
    private void UpdateNodesAvailability()
    {
        // Блокируем все ноды
        foreach (var layer in layers)
        {
            foreach (var node in layer)
            {
                node.isAvailable = false;
                UpdateNodeVisuals(node);
            }
        }

        // Если все слои посещены, выходим
        if (currentAvailableLayer < 0)
            return;

        // Делаем доступными только ноды в текущем слое и только если они еще не посещены
        foreach (var node in layers[currentAvailableLayer])
        {
            if (!node.isVisited)
            {
                node.isAvailable = true;
                UpdateNodeVisuals(node);
            }
        }
    }

    // Обновление визуального представления узла в зависимости от его состояния
    private void UpdateNodeVisuals(MapNode node)
    {
        if (node.isVisited)
        {
            // Посещенные ноды
            node.nodeImage.color = generationSettings.visitedNodeColor;
            node.nodeButton.interactable = false;
        }
        else if (node.isAvailable)
        {
            // Доступные ноды
            node.nodeImage.color = generationSettings.availableNodeColor;
            node.nodeButton.interactable = true;
        }
        else
        {
            // Недоступные ноды
            node.nodeImage.color = generationSettings.unavailableNodeColor;
            node.nodeButton.interactable = false;
        }
    }

    // Новый метод для загрузки карты из PlayerData
    public void LoadMapFromPlayerData(PlayerData playerData)
    {
        if (playerData.MapNodes == null || playerData.MapNodes.Count == 0)
        {
            Debug.Log("Нет сохраненных данных о карте. Генерируем новую карту.");
            GenerateMap();
            return;
        }

        ClearExistingNodes();
        layers.Clear();

        // Группируем ноды по слоям
        var nodesByLayer = playerData.MapNodes.GroupBy(n => n.LayerIndex).OrderBy(g => g.Key);

        foreach (var layerGroup in nodesByLayer)
        {
            List<MapNode> layerNodes = new List<MapNode>();

            foreach (MapNodeData nodeData in layerGroup)
            {
                // Находим соответствующий конфиг локации по ID
                LocationConfig locationConfig = generationSettings.locationConfigs.FirstOrDefault(
                    config => config.name == nodeData.LocationConfigId);

                if (locationConfig == null)
                {
                    Debug.LogWarning($"Конфиг локации с ID {nodeData.LocationConfigId} не найден!");
                    locationConfig = generationSettings.locationConfigs.FirstOrDefault();
                }

                // Создаем игровой объект для нода
                GameObject nodeObject = Instantiate(generationSettings.nodePrefab, generationSettings.mapContainer);
                RectTransform nodeRectTransform = nodeObject.GetComponent<RectTransform>();
                nodeRectTransform.anchoredPosition = nodeData.Position;

                // Находим тайл с указанным типом в конфиге локации
                LocationConfig.LocationTile tileConfig = locationConfig.tiles.FirstOrDefault(
                    tile => tile.tileConfig.tileType == nodeData.TileType);

                if (tileConfig == null)
                {
                    Debug.LogWarning($"Тайл типа {nodeData.TileType} не найден в конфиге локации {locationConfig.name}!");
                    tileConfig = locationConfig.tiles.FirstOrDefault();
                }

                // Создаем MapNode и настраиваем его
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

                // Устанавливаем спрайт
                if (tileConfig != null)
                {
                    mapNode.nodeImage.sprite = tileConfig.tileConfig.tileSprite;
                }

                // Устанавливаем обработчик клика
                if (nodeButton != null)
                {
                    // Используем локальную переменную для захвата mapNode
                    MapNode capturedNode = mapNode;
                    nodeButton.onClick.AddListener(() => OnNodeClick(capturedNode));
                }

                layerNodes.Add(mapNode);
            }

            layers.Add(layerNodes);
        }

        // Определяем текущий доступный слой на основе сохраненных данных
        DetermineCurrentAvailableLayer();
        UpdateNodesAvailability();
    }

    // Определяем текущий доступный слой на основе посещенных нодов
    private void DetermineCurrentAvailableLayer()
    {
        // Перебираем слои начиная с последнего
        for (int i = layers.Count - 1; i >= 0; i--)
        {
            // Если в слое есть хотя бы один посещенный нод, значит следующий слой доступен
            if (layers[i].Any(node => node.isVisited))
            {
                currentAvailableLayer = i - 1;
                return;
            }
        }

        // Если ни один нод не посещен, доступен последний слой
        currentAvailableLayer = layers.Count - 1;
    }

    // Сохраняем текущую карту в PlayerData
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
        LocationConfig selectedLocationConfig = new LocationConfig();
        for (int i = 0; i < nodesInLayer; i++)
        {
            GameObject nodeObject = Instantiate(generationSettings.nodePrefab, generationSettings.mapContainer);
            RectTransform nodeRectTransform = nodeObject.GetComponent<RectTransform>();
            float xPosition = spacing * (i + 1) - totalWidth / 2 + Random.Range(-generationSettings.nodeHorizontalSpread, generationSettings.nodeHorizontalSpread);
            nodeRectTransform.anchoredPosition = new Vector2(xPosition, layerY);
            Button nodeButton = nodeObject.GetComponent<Button>();

            // Если босс еще не был установлен и это первый слой, установим его там
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
                        continue; // Переходим к следующей итерации
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

    // Обработчик клика по ноду
    private void OnNodeClick(MapNode node)
    {
        if (!node.isAvailable)
            return;

        // Отмечаем нод как посещенный
        node.isVisited = true;
        node.isAvailable = false;

        // Отмечаем все ноды в текущем слое как недоступные
        foreach (var layerNode in layers[currentAvailableLayer])
        {
            layerNode.isAvailable = false;
        }

        // Проверяем, все ли ноды в текущем слое посещены
        bool allNodesInLayerVisited = layers[currentAvailableLayer].All(n => n.isVisited);

        // Если все ноды в текущем слое посещены, переходим к следующему слою
        if (!allNodesInLayerVisited)
        {
            // Если нет, то просто блокируем все ноды на текущем слое
            UpdateNodesAvailability();
        }

        // Переход к следующему уровню (вверх, т.е. уменьшаем индекс)
        currentAvailableLayer--;

        // Если дошли до первого слоя или выше, значит карта пройдена
        if (currentAvailableLayer < 0)
        {
            Debug.Log("Карта полностью пройдена!");
            // Можно вызвать событие завершения карты
        }
        else
        {
            // Иначе делаем доступным следующий слой
            UpdateNodesAvailability();

            // Вызываем событие о завершении слоя
            OnLayerCompleted?.Invoke(currentAvailableLayer + 1);
        }

        // Вызываем событие клика по тайлу
        OnTileClicked?.Invoke(node.tileType, node);
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
        return TileType.BattleTile;
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

    // Метод для проверки доступности узла (для внешних вызовов)
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

    // Метод для ручной активации следующего слоя (может быть полезно для тестирования)
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