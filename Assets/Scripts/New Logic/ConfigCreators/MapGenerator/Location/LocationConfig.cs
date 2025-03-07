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
    public List<LocationTile> tiles = new List<LocationTile>();
}