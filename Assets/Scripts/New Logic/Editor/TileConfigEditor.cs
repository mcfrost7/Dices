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

        reward.resource ??= new List<ResourceData>();
        EditorGUILayout.LabelField("Resources", EditorStyles.boldLabel);

        for (int i = 0; i < reward.resource.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            // Название ресурса (ObjectField)
            EditorGUILayout.LabelField($"Resource {i + 1}:", GUILayout.Width(80));

            reward.resource[i].Config = (ResourceConfig)EditorGUILayout.ObjectField(
                reward.resource[i].Config, typeof(ResourceConfig), false, GUILayout.Width(200)
            );

            // Количество
            EditorGUILayout.LabelField("Amount:", GUILayout.Width(55));
            reward.resource[i].Count = EditorGUILayout.IntField(reward.resource[i].Count, GUILayout.Width(60));

            // Кнопка удаления
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                reward.resource.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Resource"))
            reward.resource.Add(new ResourceData());

        // Опыт
        reward.expAmount = EditorGUILayout.IntField("Experience Amount", reward.expAmount);

        // Предметы
        reward.items ??= new List<ItemConfig>();
        EditorGUILayout.LabelField("Items", EditorStyles.boldLabel);
        for (int i = 0; i < reward.items.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            reward.items[i] = (ItemConfig)EditorGUILayout.ObjectField(
                $"Item {i + 1}", reward.items[i], typeof(ItemConfig), false
            );

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                reward.items.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Item"))
            reward.items.Add(null);

        if (reward.items != null && reward.items.Count > 0)
        {
            var randomItem = reward.items[Random.Range(0, reward.items.Count)];
            reward.itemInstance = new ItemInstance(randomItem);
        }
        else
        {
            reward.itemInstance = null;
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