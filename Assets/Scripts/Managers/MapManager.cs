using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using static TileConfig;
using Unity.VisualScripting;
public class MapManager : MonoBehaviour
{
    public Canvas map;
    public Camera MapCamera;
    public GameObject mapCanvas;
    private Tile[] tiles;
    public Button button;
    public Sprite[] sprites;

    private void Awake()
    {
        gameObject.SetActive(false);
    }
    void OnEnable()
    {
        mapCanvas.SetActive(true);
        map.gameObject.SetActive(true);
        CreateTiles(GameManager.Instance.player.tileConfig);
        DrawTiles();
    }
    void OnDisable()
    {
        if (mapCanvas != null)
            mapCanvas.SetActive(false);
        if (map != null )
            map.gameObject.SetActive(false);
    }
    void Start()
    {
        CreateTiles(GameManager.Instance.player.tileConfig);
        DrawTiles();
    }

    private void CreateTiles(TileConfig config)
    {
        tiles = new Tile[config.tiles.Length];

        for (int i = 0; i < config.tiles.Length; i++)
        {
            TileData tileData = config.tiles[i];
            Tile newTile;

            // Создайте тайл в зависимости от его типа
            switch (tileData.tileType)
            {
                case TileConfig.TileType.BattleTile:
                    newTile = gameObject.AddComponent<BattleTile>();
                    break;
                case TileConfig.TileType.LootTile:
                    newTile = gameObject.AddComponent<LootTile>();
                    ((LootTile)newTile).lootType = tileData.lootType; // Устанавливаем тип лута
                    break;
                case TileConfig.TileType.CampfireTile:
                    newTile = gameObject.AddComponent<CampfireTile>();
                    break;
                default:
                    Debug.LogWarning("Unknown tile type: " + tileData.tileType);
                    continue;
            }

            newTile.Initialize(tileData.level, tileData.tileName, tileData.isWalkable, tileData.isPassed);
            tiles[i] = newTile;
        }
    }
    void DrawTiles()
    {
        Tile[] sortedTiles = tiles.OrderBy(tile => tile.level).ToArray();
        // Определяем количество уровней
        int maxLevel = sortedTiles.Max(tile => tile.level);

        // Создаем массив для хранения количества объектов на каждом уровне
        int[] rowCounts = new int[maxLevel];

        foreach (Tile tile in sortedTiles)
        {
            rowCounts[tile.level - 1]++;
        }

        int previousLevel = -1;
        int y = -800; // Начальная координата Y для первого уровня

        for (int i = 0; i < sortedTiles.Length; i++)
        {
            Tile tile = sortedTiles[i];

            // Если уровень тайла изменился, обновляем Y-координату
            if (tile.level != previousLevel)
            {
                previousLevel = tile.level;
                y += 400; // Увеличиваем Y для следующего уровня
            }

            // Количество тайлов в текущем уровне
            int tilesInRow = rowCounts[tile.level - 1];

            // Расчет X-позиции с учетом расстояния 200 между тайлами
            float posX = (tilesInRow > 1) ? (i % tilesInRow) * 200 - (tilesInRow - 1) * 100 : 0;

            Vector3 position = new Vector3(posX, y, 1); // Устанавливаем X и Y по уровням
            Quaternion rotation = Quaternion.Euler(0, 0, 0); // Поворот на 0 градусов
            var instance = Instantiate(button, position, rotation);
            instance.transform.SetParent(map.transform, true);
            if (tile.isWalkable == false)
            {
                instance.transform.localScale = new Vector3(0.5f, 0.5f, 1); // Уменьшаем кнопку в два раза
            }
            else
            {
                instance.transform.localScale = Vector3.one; // Оставляем стандартный размер
            }
            // Устанавливаем спрайты и обработчики событий для кнопок
            if (tile is BattleTile)
            {
                instance.image.sprite = sprites[0];
                instance.onClick.AddListener(() =>
                {
                    GameManager.Instance.BattleTileClick(tile);
                });
            }
            else if (tile is LootTile)
            {
                instance.image.sprite = sprites[2];
                instance.onClick.AddListener(() =>
                {
                    GameManager.Instance.LootTileClick(tile);
                });
            }
            else if (tile is CampfireTile)
            {
                instance.image.sprite = sprites[1];
                instance.onClick.AddListener(() =>
                {
                    GameManager.Instance.CampfireTileClick(tile);
                });
            }
        }
    }


}
