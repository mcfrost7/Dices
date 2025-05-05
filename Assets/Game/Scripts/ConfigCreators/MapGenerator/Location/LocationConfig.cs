using UnityEngine;
using System.Collections.Generic;
using static CanvasMapGenerator;

[CreateAssetMenu(fileName = "LocationConfig", menuName = "Configs/LocationConfig")]
public class LocationConfig : ScriptableObject
{
    [System.Serializable]
    public class LocationTile
    {
        public TileType tileType;
        public List<NewTileConfig> tileConfig = new List<NewTileConfig>();
    }

    public string _locationName;
    public List<LocationTile> tiles = new List<LocationTile>();
    public int _locationLevel;
    public int minDifficulty;
    public int maxDifficulty;

    public MapGenerationSettings generationSettings = new MapGenerationSettings();
}

