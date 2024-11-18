using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileConfig", menuName = "ScriptableObjects/TileConfig")]
public class TileConfig : ScriptableObject
{
    public TileData[] tiles;

    [System.Serializable]
    public class TileData
    {
        public int level;
        public string tileName;
        public bool isWalkable;
        public bool isPassed;
        public TileType tileType; // Используем enum вместо строки
        public string lootType; // для LootTile
    }
}
