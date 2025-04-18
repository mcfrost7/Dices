using System;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Configs/Item")]

public class ItemConfig : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int power;
    public ActionType actionType;
    public ItemSideAffect sideAffect;
    [HideInInspector]public int inventoryPosition = -1;
}
[System.Serializable]
public class ItemInstance
{
    [SerializeField] private ItemConfig sourceConfig;
    public ItemConfig Config => sourceConfig;

    public string ItemName => sourceConfig?.itemName;
    public Sprite Icon => sourceConfig?.icon;
    public int Power => sourceConfig?.power ?? 0;
    public ActionType ActionType => sourceConfig?.actionType ?? ActionType.None;
    public ItemSideAffect SideAffect => sourceConfig?.sideAffect ?? ItemSideAffect.None;

    public int InventoryPosition;

    public ItemInstance(ItemConfig config)
    {
        sourceConfig = config;
        InventoryPosition = config?.inventoryPosition ?? -1;
    }

    public ItemInstance() => InventoryPosition = -1;
}


[Serializable]
public class SerializableItemConfig
{
    public string configName;
    public int inventoryPosition;

    public SerializableItemConfig(ItemInstance item)
    {
        configName = item.Config?.name;
        inventoryPosition = item.InventoryPosition;
    }

    public ItemInstance ToItemInstance()
    {
        var config = Resources.Load<ItemConfig>("ItemConfigs/" + configName);
        if (config == null)
        {
            Debug.LogWarning($"ItemConfig '{configName}' not found!");
        }
        return new ItemInstance(config) { InventoryPosition = inventoryPosition };
    }
}
