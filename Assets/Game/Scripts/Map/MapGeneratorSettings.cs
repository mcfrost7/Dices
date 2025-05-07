using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapGenerationSettings
{
    [Header("Map Layout")]
    public int numberOfLayers;
    public int minNodesPerLayer ;
    public int maxNodesPerLayer;
    public float layerHeight;


    [Header("Randomization")]
    public float nodeHorizontalSpread = 0f;
    public List<TileSpawnProbability> tileSpawnProbabilities;

    public Color availableNodeColor = Color.white;
    public Color visitedNodeColor = new Color(0.7f, 0.7f, 0.7f);
    public Color unavailableNodeColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
}
[System.Serializable]
public class TileSpawnProbability
{
    public TileType tileType;
    public float probability;
}
