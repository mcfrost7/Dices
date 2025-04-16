using UnityEngine;

public class GameDataMNG : MonoBehaviour
{
    public static GameDataMNG Instance { get; private set; }
    public PlayerData PlayerData { get => playerData; set => playerData = value; }

    [SerializeField] private CanvasMapGenerator mapGenerator;
    [SerializeField] private TeamMNG teamMNG;
    [SerializeField] private EventsController eventsController;
    [SerializeField] private ItemConfig item;

    private PlayerData playerData;
    public NewTileConfig CurrentTile { get; private set; } 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        CurrentTile = null;
    }

    public void StartNewGame()
    {
        PlayerData = new PlayerData();
        if (mapGenerator != null && teamMNG != null)
        {
            teamMNG.NewGame();
            mapGenerator.GenerateMap();
            mapGenerator.SaveMapToPlayerData();
            ResourcesMNG.Instance.SetupResources();
            ItemMNG.Instance.AddItem(new ItemInstance(item));
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
            teamMNG.LoadUnits(PlayerData);
            mapGenerator.LoadMapFromPlayerData(PlayerData);
            ResourcesMNG.Instance.SetupResources();
            ResourcesMNG.Instance.LoadResources(PlayerData);

        }
    }

    public void SaveGame()
    {
        if (PlayerData == null || mapGenerator == null || teamMNG == null)
        {
            Debug.LogWarning("Ошибка сохранения: данные отсутствуют!");
            return;
        }
        SaveLoadMNG.Save(PlayerData);
    }


    public void HandleTileClick(CanvasMapGenerator.MapNode node)
    {
        Debug.Log($"Выбран тайл типа: {node.tileType}");
        GameDataMNG.Instance.CurrentTile = node.tileConfig;
        eventsController.HandleEvent(node.tileType, node.tileConfig);
        TileUIController.Instance.ChangeUIOnTileClick();
        SaveGame();
    }

}
