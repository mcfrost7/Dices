using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player 
{
    private List<UnitStats> units = new();
    private int difficulty = 1;
    private List<Tile> tiles = new();
    private List<Resource> resources = new();
    public List<UnitStats> Units { get => units; set => units = value; }
    public int Difficulty { get => difficulty; set => difficulty = value; }
    public List<Tile> Tiles { get => tiles; set => tiles = value; }
    public List<Resource> Resources { get => resources; set => resources = value; }
}
