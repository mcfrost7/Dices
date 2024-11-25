using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Canvas map;
    [SerializeField] private GameObject mapCanvas;
    [SerializeField] private GameObject map_to_spawn_tiles;
    [SerializeField] private GameObject tiles_container; // Контейнер для тайлов
    [SerializeField] private Button button_tile_prefab; // Переименуем в buttonPrefab для ясности


    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        mapCanvas.SetActive(true);
        map.gameObject.SetActive(true);
        gameObject.GetComponent<TileManager>().LoadTilesToManager();
        DrawTiles();
    }
    private void OnDestroy()
    {
        Debug.LogError($"OnDestroy вызван на объекте {gameObject.name}. Стек вызовов:\n{System.Environment.StackTrace}");
    }

    private void OnDisable()
    {
        ClearTilesContainer();
        mapCanvas.SetActive(false);
        map.gameObject.SetActive(false);
    }

    private void DrawTiles()
    {
        Tile [] sortedTiles = GameManager.Instance.MapManager1.GetComponent<TileManager>().ActiveTiles.OrderBy(t => t.TileData.Level).ToArray();
        int[] rowCounts = CountTilesPerLevel(sortedTiles);

        int previousLevel = -1;
        float y = map_to_spawn_tiles.transform.localPosition.y;

        for (int i = 0; i < sortedTiles.Length; i++)
        {
            Tile tile = sortedTiles[i];

            if (tile.TileData.Level != previousLevel)
            {
                previousLevel = tile.TileData.Level;
                y += 200; // Увеличиваем Y для следующего уровня
            }

            CreateTileButton(tile, y, rowCounts[tile.TileData.Level - 1], i);
        }
    }

    private int[] CountTilesPerLevel(Tile[] sortedTiles)
    {
        int maxLevel = sortedTiles.Max(t => t.TileData.Level);
        int[] rowCounts = new int[maxLevel];

        foreach (Tile tile in sortedTiles)
        {
            rowCounts[tile.TileData.Level - 1]++;
        }

        return rowCounts;
    }

    private void CreateTileButton(Tile tile, float y, int tilesInRow, int index)
    {
        float posX = tilesInRow > 1 ? (index % tilesInRow) * 200 - (tilesInRow - 1) * 100 : 0;

        Vector3 position = new Vector3(posX, y, 1);
        Button instance = Instantiate(button_tile_prefab, position, Quaternion.identity, tiles_container.transform); // Установка родительского объекта
        SetButtonScale(instance, tile.TileData.IsWalkable);
        AssignSpriteAndListener(instance, tile);
    }

    private void SetButtonScale(Button instance, bool isWalkable)
    {
        instance.transform.localScale = isWalkable ? Vector3.one : new Vector3(0.5f, 0.5f, 1);
    }

    private void AssignSpriteAndListener(Button instance, Tile tile)
    {
        instance.image.sprite = tile.Tile_sprite;
        if (tile is BattleTile)
        {
            instance.onClick.AddListener(() => GameManager.Instance.BattleTileClick(tile));
        }
        else if (tile is LootTile)
        {
            instance.onClick.AddListener(() => GameManager.Instance.LootTileClick(tile));
        }
        else if (tile is CampfireTile)
        {

            instance.onClick.AddListener(() => GameManager.Instance.CampfireTileClick(tile));
        }
    }

    private void ClearTilesContainer()
    {
        if (tiles_container != null)
        {
            foreach (Transform child in tiles_container.transform)
            {
                if (child != null)
                    Destroy(child.gameObject); // Уничтожаем все кнопки в контейнере
            }
        }
    }


}