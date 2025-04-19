using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardConfig", menuName = "Configs/RewardConfig")]
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
    private ItemInstance itemInstance;

    public RewardConfig SourceConfig { get => sourceConfig; set => sourceConfig = value; }

    public SerializableRewardConfig(RewardConfig config)
    {
        SourceConfig = config;
        itemInstance = null;
    }

    public ItemInstance GetItem()
    {
        if (items != null && items.Count > 0)
        {
            var randomItem = items[Random.Range(0, items.Count)];
            return new ItemInstance(randomItem);
        }
        return null;
    }
}