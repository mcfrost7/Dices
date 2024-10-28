using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratingMap : MonoBehaviour
{
    public List<List<Tile>> tiles = new List<List<Tile>>();
    public GameObject battlePrefab; // Префаб для BattleTile
    public GameObject lootPrefab; // Префаб для LootTile
    public GameObject campfirePrefab; // Префаб для CampfireTile

    void Start()
    {
        List<Tile> row1 = new List<Tile>();
        List<Tile> row2 = new List<Tile>();
        List<Tile> row3 = new List<Tile>();

        // Инициализация объектов и добавление их в списки


        BattleTile tile1 = new BattleTile();
        tile1.Initialize(1, "Battle Tile 1", true);
        row1.Add(tile1);

        BattleTile tile2 = new BattleTile();
        tile2.Initialize(2, "Battle Tile 2", true);
        row2.Add(tile2);

        LootTile tile3 = new LootTile();
        tile3.Initialize(3, "Loot Tile 2", true, "Silver");
        row2.Add(tile3);

        LootTile tile4 = new LootTile();
        tile4.Initialize(4, "Loot Tile 3", true, "Bronze");
        row3.Add(tile4);

        CampfireTile tile5 = new CampfireTile();
        tile5.Initialize(5, "Campfire Tile 3", true);
        row3.Add(tile5);

        tiles.Add(row1);
        tiles.Add(row2);
        tiles.Add(row3);

        CreateTiles();
    }



    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Проверка нажатия левой кнопки мыши
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile != null)
                {
                    tile.OnTileClicked();
                }
            }
        }
    }

    void CreateTiles()
    {
        for (int y = 0; y < tiles.Count; y++)
        {
            List<Tile> row = tiles[y];
            for (int x = 0; x < row.Count; x++)
            {
                Tile tile = row[x];
                Vector3 position = new Vector3(x * 2, y * 2 - 2, 1); // Координаты с учетом разницы 2
                Quaternion rotation = Quaternion.Euler(0, 180, 0); // Поворот на 180 градусов по оси Y
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
                    Tile instanceTile = instance.AddComponent(tile.GetType()) as Tile;
                    if (instanceTile is LootTile lootTile)
                    {
                        lootTile.Initialize(tile.type, tile.tileName, tile.isWalkable, (tile as LootTile).lootType);
                    }
                    else
                    {
                        instanceTile.Initialize(tile.type, tile.tileName, tile.isWalkable);
                    }
                }
            }
        }
    }
}
