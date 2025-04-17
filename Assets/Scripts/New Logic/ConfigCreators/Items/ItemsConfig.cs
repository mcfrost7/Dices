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
    public int inventoryPosition = -1;
}
[System.Serializable]
public class ItemInstance
{
    public string itemName;
    public Sprite icon;
    public string iconPath;
    public int power;
    public ActionType actionType;
    public ItemSideAffect sideAffect;
    public int inventoryPosition = -1;
    private ItemConfig sourceConfig = null;

    public ItemInstance(ItemConfig config)
    {
        if (config == null) return;
        sourceConfig = config;
        iconPath = config.name;
        itemName = config.itemName;
        icon = config.icon;
        power = config.power;
        actionType = config.actionType;
        sideAffect = config.sideAffect;
        inventoryPosition = config.inventoryPosition;
    }
    public ItemInstance()
    {
        inventoryPosition = -1; 
    }
}

[Serializable]
public class SerializableItemConfig
{
    public string itemName;
    public string iconPath; 
    public int power;
    public ActionType actionType;
    public ItemSideAffect sideAffect;
    public int inventoryPosition;

    public SerializableItemConfig(ItemInstance item)
    {
        if (item == null) return;

        this.itemName = item.itemName;
        this.iconPath = "Sprites/Items/" + item.iconPath;
        this.power = item.power;
        this.power = item.power;
        this.actionType = item .actionType;
        this.sideAffect = item.sideAffect;
        this.inventoryPosition = item.inventoryPosition;
    }

    public ItemInstance ToItemInstance()
    {
        ItemInstance item = new ItemInstance();
        item.itemName = this.itemName;
        if (!string.IsNullOrEmpty(iconPath))
        {
            item.icon = Resources.Load<Sprite>(iconPath);
            if (item.icon == null)
                Debug.Log($"Icon '{iconPath}' not found in Resources!");
        }

        item.power = this.power;
        item.actionType = this.actionType;
        item.sideAffect = this.sideAffect;
        item.inventoryPosition = this.inventoryPosition;

        return item;
    }

}