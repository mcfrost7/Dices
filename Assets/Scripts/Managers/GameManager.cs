using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject BattleManager;
    public GameObject MapManager;
    public GameObject MenuManager;
    public GameObject TeamManager;
    public Canvas Canvas;
    [SerializeField] private TileConfig tileConfig;
    public Player player = new();

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


    public void EnableManager(GameObject manager, bool enable)
    {
        manager.SetActive(enable);
    }

    private void Awake()
    {
        if (player == null)
        {
            Debug.LogError("Player не назначен!");
            return;
        }
        if (tileConfig == null)
        {
            Debug.LogError("tileConfig не назначен!");
            return;
        } 
        player.tileConfig = Instantiate(tileConfig);
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        Canvas.gameObject.SetActive(true);
        EnableManager(MapManager, true);
    }
    private void OnDisable()
    {
        Canvas.gameObject.SetActive(false);
        EnableManager(MapManager, false);
        EnableManager(BattleManager, false);
        EnableManager(TeamManager, false);
    }

    public void BattleTileClick(Tile tile)
    {
        if (tile == null)
            return;
        if (tile.isWalkable == true && tile.isPassed == false)
        {
            if (IsNewGame)
            {
                EnableManager(TeamManager, true);
                EnableManager(TeamManager, false);
            }
            EnableManager(MapManager, false);
            EnableManager(BattleManager, true);
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
        foreach (var tileData in player.tileConfig.tiles)
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
        string json = JsonUtility.ToJson(player.tileConfig, true); // Конвертируем конфиг в JSON
        File.WriteAllText(Application.persistentDataPath + "/TileConfig.json", json); // Сохраняем JSON в файл
        Debug.Log("TileConfig сохранен в файл");
    }

    public void LoadTileConfigFromFile()
    {
        string path = Application.persistentDataPath + "/TileConfig.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path); // Читаем JSON из файла
            JsonUtility.FromJsonOverwrite(json, player.tileConfig); // Обновляем TileConfig
            Debug.Log("TileConfig загружен из файла");
        }
        else
        {
            Debug.LogWarning("Файл конфигурации не найден: загрузка пропущена");
        }
    }

    public void BackFromTeamToMap()
    {
        EnableManager(MapManager, true);
        EnableManager(TeamManager, false);
    }

    public void GoFromMapToTeam()
    {
        EnableManager(MapManager, false);
        EnableManager(TeamManager, true);
    }
    public void GoFromBattleToMap()
    {
        EnableManager(MapManager, true);
        EnableManager(BattleManager, false);
    }

    public void SetGameStatus(bool status)
    {
        IsNewGame = status;
    }
    public bool GetGameStatus(){ return IsNewGame; }

    public void SetActualData()
    {

    }

}
