using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "LocationMapConfig", menuName = "Map Generation/Location Map Config")]
public class LocationMapConfig : ScriptableObject
{
    [Header("Generation Settings")]
    public int MaxTotalTiles = 20;
    public float LayerHeight = 150f;
    public int MinTilesPerLayer = 1;
    public int MaxTilesPerLayer = 5;
    public float TileSeparation = 80f;
    public float LayerWidth = 400f;

    [Header("Tile Configuration")]
    public GameObject TilePrefab;
    public List<LocationConfig> LocationConfigs;
}