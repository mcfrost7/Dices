using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LocationConfig", menuName = "Configs/LocationConfig")]
public class LocationConfig : ScriptableObject
{
    [System.Serializable]
    public class LocationTile
    {
        public NewTileConfig tileConfig;
        public Vector2Int gridPosition;
    }

    public string locationName;
    public int gridWidth = 5;
    public int gridHeight = 20;
    public List<LocationTile> tiles = new List<LocationTile>();
}