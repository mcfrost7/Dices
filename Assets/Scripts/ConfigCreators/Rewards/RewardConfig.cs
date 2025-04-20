using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardConfig", menuName = "Configs/RewardConfig")]
[System.Serializable]
public class RewardConfig : ScriptableObject
{
    public List<ResourceData> resource = null;
    public int expAmount = 0;
    public List<ItemConfig> items = null;
    [HideInInspector] public ItemInstance itemInstance = new ItemInstance();
}

[System.Serializable]
public class SerializableRewardConfig
{
    [SerializeField] private RewardConfig sourceConfig;
    public List<ResourceData> resources => SourceConfig?.resource;
    public int expAmount => SourceConfig?.expAmount ?? 0;
    public List<ItemConfig> items => SourceConfig?.items;
    [HideInInspector] private ItemInstance itemInstance;
    public ItemInstance ItemInstance
    {
        get => itemInstance;
        set => itemInstance = value;
    }
    public RewardConfig SourceConfig { get => sourceConfig; set => sourceConfig = value; }

    public SerializableRewardConfig(RewardConfig config)
    {
        SourceConfig = config;
        if (config.items != null && config.items.Count > 0)
        {
            var randomItem = config.items[Random.Range(0, config.items.Count)];
            ItemInstance = new ItemInstance(randomItem);
        }
        else
        {
            ItemInstance = null;
        }
    }
    public ItemInstance GetRandomItem()
    {
        if (SourceConfig?.items != null && SourceConfig.items.Count > 0)
        {
            var randomItem = SourceConfig.items[Random.Range(0, SourceConfig.items.Count)];
            ItemInstance = new ItemInstance(randomItem);
            return ItemInstance;
        }
        return null;
    }
}