using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    private List<Tile> activeTiles = new(); // Список активных объектов тайлов
     TileConfig TileConfig1 { get; set; }
    public List<Tile> ActiveTiles { get => activeTiles; set => activeTiles = value; }

    public void Initialize(TileConfig tileConfig)
    {
        TileConfig1 = Instantiate(tileConfig);
    }
    public void LoadTilesToManager()
    {
        ClearOldTiles(); // Уничтожение старых объектов

        foreach (var tileData in TileConfig1.Tiles)
        {
            Tile newTile = CreateTile(tileData); // Создание нового объекта
            if (newTile != null)
            {
                ActiveTiles.Add(newTile); // Добавление объекта в список
            }
        }

        Debug.Log("Объекты тайлов успешно созданы.");
    }

    public void UpdateTileData(Tile clickedTile)
    {
        if (clickedTile == null)
            return;

        var clickedTileData = clickedTile.TileData;
        if (clickedTileData == null)
            return;

        int currentLevel = clickedTileData.Level;

        foreach (var tileData in TileConfig1.Tiles)
        {
            if (tileData.Level == currentLevel)
            {
                tileData.IsPassed = true; // Отметить тайл как пройденный
            }
            else if (tileData.Level == currentLevel + 1)
            {
                tileData.IsWalkable = true; // Сделать следующий тайл доступным
            }
        }
    }

    public void SaveDataFromObjects(List<TileConfig.TileData> tileDataList)
    {
        foreach (var tile in ActiveTiles)
        {
            TileConfig.TileData tileData = tile.TileData;
            if (tileData != null && tileDataList.Contains(tileData))
            {
                // Обновляем данные на основе состояния объекта
                tileData.IsWalkable = tile.TileData.IsWalkable;
                tileData.IsPassed = tile.TileData.IsPassed;
            }
        }

        Debug.Log("Данные тайлов обновлены.");
    }

    private Tile CreateTile(TileConfig.TileData tileData)
    {
        Tile newTile = tileData.TileType switch
        {
            TileType.BattleTile => gameObject.AddComponent<BattleTile>(),
            TileType.LootTile => CreateLootTile(tileData),
            TileType.CampfireTile => gameObject.AddComponent<CampfireTile>(),
            _ => null
        };

        if (newTile != null)
        {
            newTile.Initialize(tileData); // Инициализация объекта
        }

        return newTile;
    }

    private LootTile CreateLootTile(TileConfig.TileData tileData)
    {
        LootTile lootTile = gameObject.AddComponent<LootTile>();
        lootTile.Initialize(tileData, TileConfig1.Resources[0]);
        return lootTile;
    }

    private void ClearOldTiles()
    {
        // Проходим по всем активным тайлам
        foreach (var tile in ActiveTiles)
        {
            if (tile is LootTile lootTile)
            {
                Destroy(lootTile);  // Уничтожаем компонент LootTile
            }
            else if (tile is CampfireTile campfireTile)
            {
                Destroy(campfireTile);  // Уничтожаем компонент CampfireTile
            }
            else if (tile is BattleTile battleTile)
            {
                Destroy(battleTile);  // Уничтожаем компонент BattleTile
            }
        }

        // Очищаем список активных тайлов
        ActiveTiles.Clear();
    }

}
