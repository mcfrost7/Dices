using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class TileNode
{
    public LocationConfig.LocationTile Tile;
    public GameObject TileObject;
    public List<TileNode> Connections = new List<TileNode>();

    public RectTransform RectTransform => TileObject.GetComponent<RectTransform>();
    public Vector2 Position => RectTransform.anchoredPosition;
}