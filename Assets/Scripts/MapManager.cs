using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class MapManager : MonoBehaviour
{
    public GameObject map;
    public Camera MapCamera;
    public Canvas mapCanvas;
    Tile[] tiles = new Tile[6];
    public GameObject battlePrefab; // Префаб для BattleTile
    public GameObject lootPrefab; // Префаб для LootTile
    public GameObject campfirePrefab; // Префаб для CampfireTile


    void OnEnable()
    {
        mapCanvas.gameObject.SetActive(true);
        map.SetActive(true);
    }

    void OnDisable()
    {
        if (mapCanvas != null )
            mapCanvas.gameObject.SetActive(false);
        if(map != null )
            map.SetActive(false);
    }
    void Start()
    {


        BattleTile tile1 = new BattleTile();
        tile1.Initialize(1, "Battle Tile 1", true, false);
        tiles[0] = tile1;

        BattleTile tile2 = new BattleTile();
        tile2.Initialize(2, "Battle Tile 2", false, false);
        tiles[1] = tile2;

        LootTile tile3 = new LootTile();
        tile3.Initialize(2, "Loot Tile 2", false, "Silver", false);
        tiles[2] = tile3;

        LootTile tile4 = new LootTile();
        tile4.Initialize(3, "Loot Tile 3", false, "Bronze", false);
        tiles[3] = tile4;

        CampfireTile tile5 = new CampfireTile();
        tile5.Initialize(3, "Campfire Tile 3", false, false);
        tiles[4] = tile5;

        BattleTile tile6 = new BattleTile();
        tile6.Initialize(2, "Battle Tile 5", false, false);
        tiles[5] = tile6;

        DrawTiles();
    }
    void DrawTiles()
    {
        GameObject map = GameObject.Find("Map"); // Находим уже существующий объект map
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
        int y = -2;
        int tilesPerRow = 0;

        for (int x = 0; x < sortedTiles.Length; x++)
        {
            Tile tile = sortedTiles[x];

            if (tile.level != previousLevel)
            {
                previousLevel = tile.level;
                y++;
                tilesPerRow = 0;
            }

            // Расчет координат с учетом количества объектов в ряду
            int offsetX = rowCounts[tile.level - 1] - 1;
            int posX = (tilesPerRow * 2) - offsetX;

            Vector3 position = new Vector3(posX, y * 2, 1);
            Quaternion rotation = Quaternion.Euler(0, 0, 0); // Поворот на 180 градусов по оси Y
            GameObject instance = null;

            if (tile is BattleTile)
            {
                instance = Instantiate(battlePrefab, position, rotation);
            }
            else if (tile is LootTile)
            {
                instance = Instantiate(lootPrefab, position, rotation);
            }
            else if (tile is CampfireTile)
            {
                instance = Instantiate(campfirePrefab, position, rotation);
            }

            if (instance != null)
            {
                instance.transform.SetParent(map.transform); // Устанавливаем родителем map
                Tile instanceTile = instance.AddComponent(tile.GetType()) as Tile;

                if (tile.isWalkable)
                {
                    instance.transform.localScale = new Vector3(2f, 2f, 1f);
                }
                else
                {
                    instance.transform.localScale = new Vector3(1f, 1f, 1f);
                }

                if (instanceTile is LootTile lootTile)
                {
                    lootTile.Initialize(tile.level, tile.tileName, tile.isWalkable, (tile as LootTile).lootType, tile.isPassed);
                }
                else
                {
                    instanceTile.Initialize(tile.level, tile.tileName, tile.isWalkable, tile.isPassed);
                }
            }

            tilesPerRow++;
        }
    }



    public void UpdateTiles(Tile tile)
    {
        if (tile != null)
        {

            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i] == tile)
                {
                    tiles[i].isPassed = true;
                    break;
                }
            }
            int tileLevel = tile.level;

            if (tileLevel < tiles.Max(t => t.level)) // Проверяем, есть ли следующий уровень
            {
                // Ищем все тайлы следующего уровня
                foreach (Tile nextTile in tiles)
                {
                    if (nextTile.level == tileLevel + 1)
                    {
                        // Меняем isWalkable на true
                        nextTile.isWalkable = true;
                    }
                }
            }
        }
        DestroyTiles();
        DrawTiles();
    }


    public void DestroyTiles()
    {
        GameObject map = GameObject.Find("Map");
        if (map != null)
        {
            // Уничтожаем все дочерние объекты map
            foreach (Transform child in map.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

}
