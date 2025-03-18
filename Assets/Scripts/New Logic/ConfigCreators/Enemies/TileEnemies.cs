using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemies", menuName = "Configs/TileEnemies")]
public class TileEnemies : ScriptableObject
{
    [SerializeField]public  List<NewDiceConfig> newDiceConfigs;
}
