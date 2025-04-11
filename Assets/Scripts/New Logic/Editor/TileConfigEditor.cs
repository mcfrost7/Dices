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
                DrawRewardSettings(ref config.lootSettings.reward, "Loot Reward");
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
    private void DrawRewardSettings(ref RewardConfig reward, string title)
    {
        reward ??= new RewardConfig();
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

        // Инициализация списка ресурсов
        reward.resource ??= new List<ResourceData>();

        // Отображение списка ресурсов
        for (int i = 0; i < reward.resource.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            reward.resource[i].Config = (ResourceConfig)EditorGUILayout.ObjectField(
                $"Resource {i + 1}", reward.resource[i].Config, typeof(ResourceConfig), false
            );

            reward.resource[i].Count = EditorGUILayout.IntField("Amount", reward.resource[i].Count);

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                reward.resource.RemoveAt(i);
                break; // Выходим из цикла, чтобы избежать ошибок при изменении списка во время итерации
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Resource"))
        {
            reward.resource.Add(new ResourceData());
        }

        if (reward.resource.Count > 0 && GUILayout.Button("Delete Resource"))
        {
            reward.resource.RemoveAt(reward.resource.Count - 1);
        }

        // Опыт и предметы
        reward.expAmount = EditorGUILayout.IntField("Experience Amount", reward.expAmount);
        reward.item = (ItemConfig)EditorGUILayout.ObjectField("Item", reward.item, typeof(ItemConfig), false);
    }

    /// <summary>
    /// Метод для отображения списка конфигураций рулетки
    /// </summary>
    private void DrawRouletteConfigList(ref List<RouletteConfig> configs, string title)
    {
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

        // Инициализация списка конфигураций
        configs ??= new List<RouletteConfig>();

        // Отображение списка конфигураций
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
                break; // Выходим из цикла, чтобы избежать ошибок при изменении списка во время итерации
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