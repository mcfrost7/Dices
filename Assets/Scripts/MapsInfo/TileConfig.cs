using Codice.Client.Common.GameUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileConfig1", menuName = "ScriptableObjects/TileConfig1")]
public class TileConfig : ScriptableObject
{
    [SerializeField] private List<Resource> resources; 
    [SerializeField] private List<TileData> tiles;

    public List<Resource> Resources { get => resources; set => resources = value; }
    public List<TileData> Tiles { get => tiles; set => tiles = value; }

    [System.Serializable]
    public class TileData
    {
        private string tileName;
        private bool isWalkable;
        private bool isPassed;
        [SerializeField] private int level;
        [SerializeField] private TileType tileType;


        public int Level { get => level; set => level = value; }
        public string TileName { get => tileName; set => tileName = value; }
        public bool IsWalkable { get => isWalkable; set => isWalkable = value; }
        public bool IsPassed { get => isPassed; set => isPassed = value; }
        public TileType TileType { get => tileType; set => tileType = value; }
    }
}
