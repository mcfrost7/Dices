using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public GameObject BattleManager;
    public GameObject MapManager;
    public GameObject MenuManager;
    public GameObject TeamManager;
    public Canvas Canvas;
    [SerializeField] private TileConfig tileConfig;
    public TileConfig tempConfig;

    private static GameManager instance;

    public static GameManager Instance {  
        get { if (instance == null) 
            { 
                instance = FindAnyObjectByType<GameManager>();
            }
            return instance; } 
    }

    public void EnableManager(GameObject manager, bool enable)
    {
        manager.SetActive(enable);
    }

    private void Awake()
    {
        tempConfig = Instantiate(tileConfig);
    }
    private void OnEnable()
    {
        Canvas.gameObject.SetActive(true);
        EnableManager(MapManager, true);
    }

    public void BattleTileClick(Tile tile)
    {
        if (tile == null)
            return;
        if (tile.isWalkable == true && tile.isPassed == false)
        {
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
        foreach (var tileData in tempConfig.tiles)
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
        string json = JsonUtility.ToJson(tempConfig, true); // Конвертируем конфиг в JSON
        File.WriteAllText(Application.persistentDataPath + "/TileConfig.json", json); // Сохраняем JSON в файл
        Debug.Log("TileConfig сохранен в файл");
    }

    public void LoadTileConfigFromFile()
    {
        string path = Application.persistentDataPath + "/TileConfig.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path); // Читаем JSON из файла
            JsonUtility.FromJsonOverwrite(json, tempConfig); // Обновляем TileConfig
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
        EnableManager(MapManager,false);
        EnableManager(TeamManager,true);
    }
}
