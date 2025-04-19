using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(NewTileConfig))]
public class TileConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NewTileConfig config = (NewTileConfig)target;

        // Основные параметры
        config.tileSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", config.tileSprite, typeof(Sprite), false);
        config.tileType = (TileType)EditorGUILayout.EnumPopup("Tile Type", config.tileType);

        // Переключение по типам тайлов
        switch (config.tileType)
        {
            case TileType.BattleTile:
                config.battleSettings ??= new BattleSettings();
                config.battleSettings.battleDifficulty = EditorGUILayout.IntField("Battle Difficulty", config.battleSettings.battleDifficulty);
                config.battleSettings.tileEnemies = (TileEnemies)EditorGUILayout.ObjectField("Enemy Race", config.battleSettings.tileEnemies, typeof(TileEnemies), false);
                DrawRewardSettings(ref config.battleSettings.reward, "Battle Reward");
                break;

            case TileType.LootTile:
                config.lootSettings ??= new LootSettings();
                DrawRewardSettings( ref config.lootSettings.reward, "Loot Reward");
                break;

            case TileType.CampTile:
                config.campSettings ??= new CampSettings();
                config.campSettings.healAmount = EditorGUILayout.IntField("Heal Amount", config.campSettings.healAmount);
                config.campSettings.isUpgradeAvailable = EditorGUILayout.Toggle("Upgrade Available", config.campSettings.isUpgradeAvailable);
                config.campSettings.isReinforceAvailable = EditorGUILayout.Toggle("Reinforce Available", config.campSettings.isReinforceAvailable);
                config.campSettings.isShopAvailable = EditorGUILayout.Toggle("Shop Available", config.campSettings.isShopAvailable);
                break;

            case TileType.RouleteTile:
                config.rouleteSettings ??= new RouleteSettings();
                DrawRouletteConfigList(ref config.rouleteSettings._configs, "Roulette Configs");
                break;

            case TileType.BossTile:
                config.bossSettings ??= new BossSettings();
                config.bossSettings.battleDifficulty = EditorGUILayout.IntField("Battle Difficulty", config.bossSettings.battleDifficulty);
                config.bossSettings.tileEnemies = (TileEnemies)EditorGUILayout.ObjectField("Enemy Race", config.bossSettings.tileEnemies, typeof(TileEnemies), false);
                DrawRewardSettings(ref config.bossSettings.reward, "Boss Reward");
                break;
        }

        EditorUtility.SetDirty(config); // Обновляем данные в инспекторе
    }

    /// <summary>
    /// Метод для удобного отображения настроек наград
    /// </summary>

    private void DrawRewardSettings(ref SerializableRewardConfig reward, string title)
    {
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Load Config:", GUILayout.Width(80));

        RewardConfig currentConfig = reward?.SourceConfig;
        RewardConfig newConfig = (RewardConfig)EditorGUILayout.ObjectField(
            currentConfig,
            typeof(RewardConfig),
            false,
            GUILayout.Width(200)
        );

        EditorGUILayout.EndHorizontal();

        // Обновляем reward только если конфиг изменился
        if (newConfig != currentConfig && newConfig != null)
        {
            reward = new SerializableRewardConfig(newConfig);
        }

        // Показываем содержимое
        if (reward?.SourceConfig != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Reward Config Preview:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Exp Amount: {reward.expAmount}");
            EditorGUILayout.LabelField($"Resources: {reward.resources?.Count ?? 0}");
            EditorGUILayout.LabelField($"Items: {reward.items?.Count ?? 0}");
            EditorGUILayout.LabelField($"ItemInstance: {reward.GetItem()?.Config?.name ?? "None"}");
        }
    }




    private void DrawRouletteConfigList(ref List<RouletteConfig> configs, string title)
    {
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        configs ??= new List<RouletteConfig>();
        for (int i = 0; i < configs.Count; i++)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField($"Roulette Config {i + 1}", EditorStyles.boldLabel);

            configs[i] = (RouletteConfig)EditorGUILayout.ObjectField(
                "Config", configs[i], typeof(RouletteConfig), false
            );

            if (GUILayout.Button("Remove Config"))
            {
                configs.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Add Roulette Config"))
        {
            configs.Add(null);
        }
    }
}