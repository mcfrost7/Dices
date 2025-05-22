using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

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
    public EnemyType enemyType;
    public List<LocationTile> tiles;
    public int _locationLevel;
    public int minDifficulty;
    public int maxDifficulty;
    public List<Sprite> battleBack;

    public MapGenerationSettings generationSettings = new MapGenerationSettings();
}

[System.Serializable]
public class SerializableLocation
{
    public string locationName;
    public string _configName;
    public int locationLevel;
    public int minDifficulty;
    public int maxDifficulty;
    public List<TileData> tiles = new List<TileData>();
    public List<string> battleBackSpritePaths;
    public MapGenerationSettings generationSettings;

    public SerializableLocation(LocationConfig config)
    {
        this._configName = config.name;
        this.locationName = config._locationName;
        this.locationLevel = config._locationLevel;
        this.minDifficulty = config.minDifficulty;
        this.maxDifficulty = config.maxDifficulty;
        this.battleBackSpritePaths = new List<string>();
        foreach (var sprite in config.battleBack)
        {
            this.battleBackSpritePaths.Add("Sprites/Backgrounds/"+ sprite.name); 
        }

        foreach (var tile in config.tiles)
        {
            this.tiles.Add(new TileData
            {
                tileType = tile.tileType,
                tileConfig = tile.tileConfig
            });
        }
        this.generationSettings = config.generationSettings;
    }

}

[System.Serializable]
public class TileData
{
    public TileType tileType;
    public List<NewTileConfig> tileConfig;
}
