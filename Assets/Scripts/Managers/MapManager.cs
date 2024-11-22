using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Canvas map;
    [SerializeField] private GameObject mapCanvas;
    [SerializeField] private Sprite[] sprites;
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
        gameObject.GetComponent<TileManager>().LoadTilesToPlayer();
        DrawTiles();
    }

    private void OnDisable()
    {
        ClearTilesContainer();
        mapCanvas.SetActive(false);
        map.gameObject.SetActive(false);
    }

    private void DrawTiles()
    {
        Tile[] sortedTiles = GameManager.Instance.Player.Tiles.OrderBy(t => t.level).ToArray();
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