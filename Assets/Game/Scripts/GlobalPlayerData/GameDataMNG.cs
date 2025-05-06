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
    public CanvasMapGenerator MapGenerator { get => mapGenerator; set => mapGenerator = value; }

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
        if (MapGenerator != null && teamMNG != null)
        {
            teamMNG.NewGame();
            playerData.LocationLevel = 1;
            MapGenerator.GenerateMap(playerData.LocationLevel);
            MapGenerator.SaveMapToPlayerData();
            ResourcesMNG.Instance.SetupResources();
            SaveGame();
        }
    }


    public bool LoadGame()
    {
        if (!SaveLoadMNG.SaveExists())
        {
            Debug.LogWarning("Сохранение не найдено!");
            return false;
        }

        PlayerData = SaveLoadMNG.Load();
        if (MapGenerator != null && teamMNG != null)
        {
            teamMNG.LoadUnits(PlayerData);
            MapGenerator.LoadMapFromPlayerData(PlayerData);
            ResourcesMNG.Instance.SetupResources();
            ResourcesMNG.Instance.LoadResources(PlayerData);

        }
        return true;    
    }

    public void SaveGame()
    {
        if (PlayerData == null || MapGenerator == null || teamMNG == null)
        {
            Debug.LogWarning("Ошибка сохранения: данные отсутствуют!");
            return;
        }
        SaveLoadMNG.Save(PlayerData);
    }

    public void DeleteGame()
    {
        SaveLoadMNG.DeleteSave();
    }
    public void HandleTileClick(CanvasMapGenerator.MapNode node)
    {
        CurrentTile = node.tileConfig;
        eventsController.HandleEvent(node);
    }

}
