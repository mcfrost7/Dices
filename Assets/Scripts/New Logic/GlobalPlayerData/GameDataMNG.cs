using UnityEngine;

public class GameDataMNG : MonoBehaviour
{
    public static GameDataMNG Instance { get; private set; }
    public PlayerData PlayerData { get => playerData; set => playerData = value; }

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
        PlayerData = new PlayerData();
        if (mapGenerator != null && teamMNG != null)
        {
            teamMNG.NewGame();
            mapGenerator.GenerateMap();
            mapGenerator.SaveMapToPlayerData(PlayerData);
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

        PlayerData = SaveLoadMNG.Load();
        if (mapGenerator != null && teamMNG != null)
        {
            teamMNG.LoadUnits();
            mapGenerator.LoadMapFromPlayerData(PlayerData);
        }
    }

    public void SaveGame()
    {
        if (PlayerData == null || mapGenerator == null || teamMNG == null)
        {
            Debug.LogWarning("Ошибка сохранения: данные отсутствуют!");
            return;
        }

        teamMNG.SaveUnits();

        // Сохраняем карту
        mapGenerator.SaveMapToPlayerData(PlayerData);
        SaveLoadMNG.Save(PlayerData);
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
