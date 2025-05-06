using UnityEngine;
using System.Collections.Generic;

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
    public List<LocationTile> tiles;
    public int _locationLevel;
    public int minDifficulty;
    public int maxDifficulty;
    public List<Sprite> battleBack;

    public MapGenerationSettings generationSettings = new MapGenerationSettings();
}

