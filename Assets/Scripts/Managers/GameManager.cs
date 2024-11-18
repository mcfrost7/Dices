using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]private GameObject BattleManager;
    [SerializeField] private GameObject MapManager;
    [SerializeField] private GameObject MenuManager;
    [SerializeField] private GameObject TeamManager;
    [SerializeField] private Canvas Canvas;
    [SerializeField] private TileConfig tileConfig;
    private Player player = new();

    private bool IsNewGame;

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<GameManager>();
            }
            return instance;
        }
    }

    public GameObject BattleManager1 { get => BattleManager; set => BattleManager = value; }
    public GameObject MapManager1 { get => MapManager; set => MapManager = value; }
    public GameObject MenuManager1 { get => MenuManager; set => MenuManager = value; }
    public GameObject TeamManager1 { get => TeamManager; set => TeamManager = value; }
    public Canvas Canvas1 { get => Canvas; set => Canvas = value; }
    public TileConfig TileConfig { get => tileConfig; set => tileConfig = value; }
    public Player Player { get => player; set => player = value; }
    public bool IsNewGame1 { get => IsNewGame; set => IsNewGame = value; }

    public void EnableManager(GameObject manager, bool enable)
    {
        manager.SetActive(enable);
    }

    private void Awake()
    {
        if (Player == null)
        {
            Debug.LogError("Player не назначен!");
            return;
        }
        if (TileConfig == null)
        {
            Debug.LogError("tileConfig не назначен!");
            return;
        } 
        Player.tileConfig = Instantiate(TileConfig);
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        Canvas1.gameObject.SetActive(true);
        EnableManager(MapManager1, true);
    }
    private void OnDisable()
    {
        if (Canvas1 != null) Canvas1.gameObject.SetActive(false);
        EnableManager(MapManager1, false);
        EnableManager(BattleManager1, false);
        EnableManager(TeamManager1, false);
    }

    public void BattleTileClick(Tile tile)
    {
        if (tile == null)
            return;
        if (tile.isWalkable == true && tile.isPassed == false)
        {
            if (IsNewGame1)
            {
                EnableManager(TeamManager1, true);
                EnableManager(TeamManager1, false);
            }
            EnableManager(MapManager1, false);
            EnableManager(BattleManager1, true);
            UpdateConfig(tile);
            //SaveTileConfigToFile();
            //LoadTileConfigFromFile();
        }

    }
    public void LootTileClick(Tile tile)
    {
        if (tile == null)
            return;
        if (tile.isWalkable == true && tile.isPassed == false)
        {
            Debug.Log("loot");
        }

    }
    public void CampfireTileClick(Tile tile)
    {
        if (tile == null)
            return;
        if (tile.isWalkable == true && tile.isPassed == false)
        {
            Debug.Log("campfire");
        }


    }

    public void UpdateConfig(Tile clickedTile)
    {
        // Получаем уровень и имя нажатого тайла
        int currentLevel = clickedTile.level;
        string clickedTileName = clickedTile.tileName;

        // Проходим по массиву тайлов в конфиге и обновляем их
        foreach (var tileData in Player.tileConfig.tiles)
        {
            // Если тайл совпадает с нажатым, обновляем isPassed
            if (tileData.level == currentLevel && tileData.tileName == clickedTileName)
            {
                tileData.isPassed = true;
            }

            // Делаем все тайлы следующего уровня доступными для прохода
            if (tileData.level == currentLevel + 1)
            {
                tileData.isWalkable = true;
            }
        }

    }
    public void SaveTileConfigToFile()
    {
        string json = JsonUtility.ToJson(Player.tileConfig, true); // Конвертируем конфиг в JSON
        File.WriteAllText(Application.persistentDataPath + "/TileConfig.json", json); // Сохраняем JSON в файл
        Debug.Log("TileConfig сохранен в файл");
    }

    public void LoadTileConfigFromFile()
    {
        string path = Application.persistentDataPath + "/TileConfig.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path); // Читаем JSON из файла
            JsonUtility.FromJsonOverwrite(json, Player.tileConfig); // Обновляем TileConfig
            Debug.Log("TileConfig загружен из файла");
        }
        else
        {
            Debug.LogWarning("Файл конфигурации не найден: загрузка пропущена");
        }
    }

    public void BackFromTeamToMap()
    {
        EnableManager(MapManager1, true);
        EnableManager(TeamManager1, false);
    }

    public void GoFromMapToTeam()
    {
        EnableManager(MapManager1, false);
        EnableManager(TeamManager1, true);
    }
    public void GoFromBattleToMap()
    {
        EnableManager(MapManager1, true);
        EnableManager(BattleManager1, false);
    }

    public void SetGameStatus(bool status)
    {
        IsNewGame1 = status;
    }
    public bool GetGameStatus(){ return IsNewGame1; }

    public void SetActualData()
    {

    }

}
