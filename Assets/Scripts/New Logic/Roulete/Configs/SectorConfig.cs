using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public class SectorConfig
{
    public string Name;
    [Range(0, 1)] public float Percent;
    public Color Color = Color.white;
    public List<RewardConfig> WinItems;
}
