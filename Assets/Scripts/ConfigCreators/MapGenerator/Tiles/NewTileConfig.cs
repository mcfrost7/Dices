using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTileConfig", menuName = "Configs/NewTileConfig")]
public class NewTileConfig : ScriptableObject
{
    public TileType tileType;
    public Sprite tileSprite;
    public BattleSettings battleSettings;
    public LootSettings lootSettings;
    public CampSettings campSettings;
    public RouleteSettings rouleteSettings;
    public BossSettings bossSettings;
}

[System.Serializable]
public class BattleSettings
{
    public int battleDifficulty; 
    public TileEnemies tileEnemies;
    public SerializableRewardConfig reward = null;
}

[System.Serializable]
public class LootSettings
{
    public SerializableRewardConfig reward;
}

[System.Serializable]
public class CampSettings
{
    public int healAmount; // Сколько восстанавливает
    public bool isUpgradeAvailable; 
    public bool isReinforceAvailable;
    public bool isShopAvailable;
}

[System.Serializable]
public class RouleteSettings
{
    public List<RouletteConfig> _configs;
}

[System.Serializable]
public class BossSettings
{
    public int battleDifficulty;
    public TileEnemies tileEnemies;
    public SerializableRewardConfig reward;
}
