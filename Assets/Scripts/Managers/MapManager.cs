using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Canvas map;
    [SerializeField] private GameObject mapCanvas;
    [SerializeField] private GameObject tiles_container; // Контейнер для тайлов
    [SerializeField] private Button button_tile_prefab; // Переименуем в buttonPrefab для ясности
    [SerializeField] private Sprite[] sprites;

    private List<Tile> tiles = new List<Tile>(); // Используем список для хранения тайлов

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        mapCanvas.SetActive(true);
        map.gameObject.SetActive(true);
        CreateTiles(GameManager.Instance.player.tileConfig);
        DrawTiles();
    }

    private void OnDisable()
    {
        ClearTilesContainer();
        mapCanvas.SetActive(false);
        map.gameObject.SetActive(false);
    }

    private void CreateTiles(TileConfig config)
    {
        ClearOldTiles();
        tiles.Clear(); // Очищаем список перед созданием новых тайлов

        foreach (TileConfig.TileData tileData in config.tiles)
        {
            tiles.Add(CreateTile(tileData)); // Добавляем новые тайлы в список
        }
    }

    private void ClearOldTiles()
    {
        foreach (var tile in tiles)
        {
            if (tile != null)
            {
                Destroy(tile);
            }
        }
        tiles.Clear(); // Очищаем список тайлов
    }

    private Tile CreateTile(TileConfig.TileData tileData)
    {
        Tile newTile = tileData.tileType switch
        {
            TileConfig.TileType.BattleTile => gameObject.AddComponent<BattleTile>(),
            TileConfig.TileType.LootTile => CreateLootTile(tileData),
            TileConfig.TileType.CampfireTile => gameObject.AddComponent<CampfireTile>(),
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

    private void DrawTiles()
    {
        Tile[] sortedTiles = tiles.OrderBy(t => t.level).ToArray();
        int[] rowCounts = CountTilesPerLevel(sortedTiles);

        int previousLevel = -1;
        int y = -800;

        for (int i = 0; i < sortedTiles.Length; i++)
        {
            Tile tile = sortedTiles[i];

            if (tile.level != previousLevel)
            {
                previousLevel = tile.level;
                y += 400; // Увеличиваем Y для следующего уровня
            }

            CreateTileButton(tile, y, rowCounts[tile.level - 1], i);
        }
    }

    private int[] CountTilesPerLevel(Tile[] sortedTiles)
    {
        int maxLevel = sortedTiles.Max(t => t.level);
        int[] rowCounts = new int[maxLevel];

        foreach (Tile tile in sortedTiles)
        {
            rowCounts[tile.level - 1]++;
        }

        return rowCounts;
    }

    private void CreateTileButton(Tile tile, int y, int tilesInRow, int index)
    {
        float posX = tilesInRow > 1 ? (index % tilesInRow) * 200 - (tilesInRow - 1) * 100 : 0;

        Vector3 position = new Vector3(posX, y, 1);
        Button instance = Instantiate(button_tile_prefab, position, Quaternion.identity, tiles_container.transform); // Установка родительского объекта
        SetButtonScale(instance, tile.isWalkable);
        AssignSpriteAndListener(instance, tile);
    }

    private void SetButtonScale(Button instance, bool isWalkable)
    {
        instance.transform.localScale = isWalkable ? Vector3.one : new Vector3(0.5f, 0.5f, 1);
    }

    private void AssignSpriteAndListener(Button instance, Tile tile)
    {
        if (tile is BattleTile)
        {
            instance.image.sprite = sprites[0];
            instance.onClick.AddListener(() => GameManager.Instance.BattleTileClick(tile));
        }
        else if (tile is LootTile)
        {
            instance.image.sprite = sprites[2];
            instance.onClick.AddListener(() => GameManager.Instance.LootTileClick(tile));
        }
        else if (tile is CampfireTile)
        {
            instance.image.sprite = sprites[1];
            instance.onClick.AddListener(() => GameManager.Instance.CampfireTileClick(tile));
        }
    }

    private void ClearTilesContainer()
    {
        foreach (Transform child in tiles_container.transform)
        {
            Destroy(child.gameObject); // Уничтожаем все кнопки в контейнере
        }
    }
}