using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    public TileConfig TileConfig1 { get; set; }


    public void Initialize(TileConfig tileConfig)
    {
        TileConfig1 = Instantiate(tileConfig);
    }

    public void UpdateTileConfig(Tile clickedTile)
    {
        if (clickedTile == null)
            return;

        int currentLevel = clickedTile.level;
        string clickedTileName = clickedTile.tileName;

        foreach (var tileData in TileConfig1.tiles)
        {
            if (tileData.level == currentLevel && tileData.tileName == clickedTileName)
            {
                tileData.isPassed = true;
            }
            if (tileData.level == currentLevel + 1)
            {
                tileData.isWalkable = true;
            }
        }
    }

    public void SaveTileConfigToFile()
    {
        string json = JsonUtility.ToJson(TileConfig1, true);
        File.WriteAllText(Application.persistentDataPath + "/TileConfig1.json", json);
        Debug.Log("TileConfig1 сохранен в файл");
    }

    public void LoadTileConfigFromFile()
    {
        string path = Application.persistentDataPath + "/TileConfig1.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, TileConfig1);
            Debug.Log("TileConfig1 загружен из файла");
        }
        else
        {
            Debug.LogWarning("Файл конфигурации не найден: загрузка пропущена");
        }
    }

    public void LoadTilesToPlayer()
    {
        // Очистка текущих тайлов игрока
        GameManager.Instance.Player.Tiles.Clear();

        // Перебор всех данных тайлов в конфиге
        foreach (TileConfig.TileData tileData in TileConfig1.tiles)
        {
            // Создание тайла из данных TileData
            Tile newTile = CreateTile(tileData);

            // Добавление созданного тайла в список Tiles игрока
            GameManager.Instance.Player.Tiles.Add(newTile);
        }

        Debug.Log("Тайлы успешно загружены в Player.");
    }

    private Tile CreateTile(TileConfig.TileData tileData)
    {
        Tile newTile = tileData.tileType switch
        {
            TileType.BattleTile => gameObject.AddComponent<BattleTile>(),
            TileType.LootTile => CreateLootTile(tileData),
            TileType.CampfireTile => gameObject.AddComponent<CampfireTile>(),
            _ => throw new System.ArgumentException("Unknown tile type: " + tileData.tileType)
        };

        newTile.Initialize(tileData.level, tileData.tileName, tileData.isWalkable, tileData.isPassed);
        return newTile;
    }
    private LootTile CreateLootTile(TileConfig.TileData tileData)
    {
        LootTile lootTile = gameObject.AddComponent<LootTile>();
        lootTile.lootType = tileData.lootType; // Устанавливаем тип лута
        return lootTile;
    }

    private void ClearOldTiles()
    {
        foreach (var tile in GameManager.Instance.Player.Tiles)
        {
            if (tile != null)
            {
                Destroy(tile);
            }
        }
        GameManager.Instance.Player.Tiles.Clear(); // Очищаем список тайлов
    }

}
