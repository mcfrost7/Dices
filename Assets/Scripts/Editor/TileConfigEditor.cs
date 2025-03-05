using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NewTileConfig))]
public class TileConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NewTileConfig config = (NewTileConfig)target;

        // Отображаем спрайт и тип тайла
        config.tileSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", config.tileSprite, typeof(Sprite), false);
        config.tileType = (TileType)EditorGUILayout.EnumPopup("Tile Type", config.tileType);

        // В зависимости от типа тайла отображаем нужные параметры
        switch (config.tileType)
        {
            case TileType.BattleTile:
                if (config.battleSettings == null) config.battleSettings = new BattleSettings();

                config.battleSettings.battleDifficulty = EditorGUILayout.IntField("Battle Difficulty", config.battleSettings.battleDifficulty);
                config.battleSettings.typesInfo = (TypesInfo)EditorGUILayout.ObjectField("Enemy Race", config.battleSettings.typesInfo, typeof(TypesInfo), false);

                // Награды
                if (config.battleSettings.reward == null) config.battleSettings.reward = new RewardConfig();
                EditorGUILayout.LabelField("Reward Settings", EditorStyles.boldLabel);
                config.battleSettings.reward.resourceAmount = EditorGUILayout.IntField("Resource Amount", config.battleSettings.reward.resourceAmount);
                config.battleSettings.reward.expAmount = EditorGUILayout.IntField("Experience Amount", config.battleSettings.reward.expAmount);
                config.battleSettings.reward.item = (ItemConfig)EditorGUILayout.ObjectField("Item", config.battleSettings.reward.item, typeof(ItemConfig), false);
                break;

            case TileType.LootTile:
                // Награды
                if (config.lootSettings.reward == null) config.lootSettings.reward = new RewardConfig();
                EditorGUILayout.LabelField("Reward Settings", EditorStyles.boldLabel);
                config.lootSettings.reward.resourceAmount = EditorGUILayout.IntField("Resource Amount", config.lootSettings.reward.resourceAmount);
                config.lootSettings.reward.expAmount = EditorGUILayout.IntField("Experience Amount", config.lootSettings.reward.expAmount);
                config.lootSettings.reward.item = (ItemConfig)EditorGUILayout.ObjectField("Item", config.lootSettings.reward.item, typeof(ItemConfig), false);
                break;

            case TileType.CampTile:
                if (config.campSettings == null) config.campSettings = new CampSettings();
                config.campSettings.healAmount = EditorGUILayout.IntField("Heal Amount", config.campSettings.healAmount);
                config.campSettings.isUpgradeAvailable = EditorGUILayout.Toggle("Upgrade Available", config.campSettings.isUpgradeAvailable);
                config.campSettings.isReinforceAvailable = EditorGUILayout.Toggle("ReinforceAvailable", config.campSettings.isReinforceAvailable);
                config.campSettings.isShopAvailable = EditorGUILayout.Toggle("ShopAvailable", config.campSettings.isShopAvailable);
                break;

            case TileType.RouleteTile:
                if (config.rouleteSettings == null) config.rouleteSettings = new RouleteSettings();
                EditorGUILayout.LabelField("Roulete Reward Settings", EditorStyles.boldLabel);
                config.rouleteSettings.reward.resourceAmount = EditorGUILayout.IntField("Resource Amount", config.lootSettings.reward.resourceAmount);
                config.rouleteSettings.reward.expAmount = EditorGUILayout.IntField("Experience Amount", config.lootSettings.reward.expAmount);
                config.lootSettings.reward.item = (ItemConfig)EditorGUILayout.ObjectField("Item", config.lootSettings.reward.item, typeof(ItemConfig), false);
                break;

            case TileType.BossTile:
                if (config.bossSettings == null) config.bossSettings = new BossSettings();

                config.bossSettings.battleDifficulty = EditorGUILayout.IntField("Battle Difficulty", config.bossSettings.battleDifficulty);
                config.bossSettings.typesInfo = (TypesInfo)EditorGUILayout.ObjectField("Enemy Race", config.bossSettings.typesInfo, typeof(TypesInfo), false);

                EditorGUILayout.LabelField("Boss Reward Settings", EditorStyles.boldLabel);
                config.bossSettings.reward.resourceAmount = EditorGUILayout.IntField("Resource Amount", config.bossSettings.reward.resourceAmount);
                config.bossSettings.reward.expAmount = EditorGUILayout.IntField("Experience Amount", config.bossSettings.reward.expAmount);
                config.bossSettings.reward.item = (ItemConfig)EditorGUILayout.ObjectField("Item", config.bossSettings.reward.item, typeof(ItemConfig), false);
                break;
        }

        EditorUtility.SetDirty(config); // Обновляем данные в инспекторе
    }
}
