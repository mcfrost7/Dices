using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player 
{
    private List<UnitStats> units = new();
    private int difficulty = 1;
    private List<TileConfig.TileData> tiles = new();
    private List<Resource> resources = new();
    private List<ItemConfig> items = new();
    public List<UnitStats> Units { get => units; set => units = value; }
    public int Difficulty { get => difficulty; set => difficulty = value; }
    public List<TileConfig.TileData> Tiles { get => tiles; set => tiles = value; }
    public List<Resource> Resources { get => resources; set => resources = value; }

    public List<ItemConfig> Items {  get => items; set => items = value;}
}
