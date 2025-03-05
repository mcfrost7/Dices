using UnityEngine;

[CreateAssetMenu(fileName = "NewTileConfig", menuName = "Configs/NewTileConfig")]
public class NewTileConfig : ScriptableObject
{
    public TileType tileType;
    public Sprite tileSprite; // Спрайт для этого типа тайла
    public BattleSettings battleSettings;
    public LootSettings lootSettings;
    public CampSettings campSettings;
    public RouleteSettings rouleteSettings;
    public BossSettings bossSettings;
}

// **Настройки для BattleTile**
[System.Serializable]
public class BattleSettings
{
    public int battleDifficulty;  // Сложность боя
    public TypesInfo typesInfo; // раса врагов
    public RewardConfig reward; // награда
}

// **Настройки для LootTile**
[System.Serializable]
public class LootSettings
{
    public RewardConfig reward;
}

// **Настройки для CampTile**
[System.Serializable]
public class CampSettings
{
    public int healAmount; // Сколько восстанавливает
    public bool isUpgradeAvailable; 
    public bool isReinforceAvailable;
    public bool isShopAvailable;
}

// **Настройки для RouleteTile**
[System.Serializable]
public class RouleteSettings
{
    public RewardConfig reward;  
}

// **Настройки для BossTile**
[System.Serializable]
public class BossSettings
{
    public int battleDifficulty;  // Сложность боя
    public TypesInfo typesInfo; // раса врагов
    public RewardConfig reward; // награда
}
