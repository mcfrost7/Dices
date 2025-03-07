using UnityEngine;

public class MapMNG : MonoBehaviour
{
    public static MapMNG Instance { get; private set; }

    [SerializeField] private CanvasMapGenerator mapGenerator;
    [SerializeField] private TeamMNG teamMNG;

    private PlayerData playerData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartNewGame()
    {
        playerData = new PlayerData(); // Новый пустой профиль

        if (teamMNG != null)
        {
            teamMNG.NewGame();
        }
        if (mapGenerator != null)
        {
            mapGenerator.GenerateMap();
            mapGenerator.SaveMapToPlayerData(playerData);
            SaveGame();
        }
    }


    public void LoadGame()
    {
        if (!SaveLoadMNG.SaveExists())
        {
            Debug.LogWarning("Сохранение не найдено!");
            return;
        }

        playerData = SaveLoadMNG.Load();

        if (teamMNG != null)
        {
            teamMNG.LoadUnits();
        }
        if (mapGenerator != null)
        {
            mapGenerator.LoadMapFromPlayerData(playerData);
        }
    }

    public void SaveGame()
    {
        if (playerData == null || mapGenerator == null || teamMNG == null)
        {
            Debug.LogWarning("Ошибка сохранения: данные отсутствуют!");
            return;
        }

        teamMNG.SaveUnits();

        // Сохраняем карту
        mapGenerator.SaveMapToPlayerData(playerData);
        SaveLoadMNG.Save(playerData);
    }

    public void InitializeEventHandlers()
    {
        if (mapGenerator != null)
        {
            mapGenerator.OnTileClicked.AddListener(HandleTileClick);
            mapGenerator.OnLayerCompleted.AddListener(HandleLayerCompleted);
        }
    }

    private void HandleTileClick(TileType tileType, CanvasMapGenerator.MapNode node)
    {
        Debug.Log($"Выбран тайл типа: {tileType}");

        switch (tileType)
        {
            case TileType.BattleTile:
                Debug.Log("Инициализация боя!");
                break;
            case TileType.CampTile:
                Debug.Log("Посещён фортпост!");
                break;
            case TileType.BossTile:
                Debug.Log("Инициализация битвы с боссом!");
                break;
            case TileType.RouleteTile:
                Debug.Log("Рулетка!");
                break;
            case TileType.LootTile:
                Debug.Log("Ресурсы!");
                break;
        }
        SaveGame();
    }

    private void HandleLayerCompleted(int completedLayerIndex)
    {
        Debug.Log($"Слой {completedLayerIndex} завершен!");
        SaveGame();
    }

    [ContextMenu("Test Save")]
    private void TestSave()
    {
        SaveGame();
        Debug.Log("Тестовое сохранение выполнено");
    }

    [ContextMenu("Test Load")]
    private void TestLoad()
    {
        LoadGame();
        Debug.Log("Тестовая загрузка выполнена");
    }

    [ContextMenu("Test New Game")]
    private void TestNewGame()
    {
        StartNewGame();
        Debug.Log("Создана новая игра");
    }
}
