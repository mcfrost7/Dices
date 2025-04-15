using System;
using UnityEngine;
using static UnityEditor.Progress;

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
    public int power;
    public Sprite icon;
    public ActionType actionType;
    public ItemSideAffect sideAffect;
    public int inventoryPosition = -1;

    public ItemInstance(ItemConfig config)
    {
        itemName = config.itemName;
        power = config.power;
        icon = config.icon;
        actionType = config.actionType;
        sideAffect = config.sideAffect;
        inventoryPosition = config.inventoryPosition;
    }
}

[Serializable]
public class SerializableItemConfig
{
    public string itemName;
    public string iconPath; // Path to the sprite asset
    public int power;
    public ActionType actionType;
    public ItemSideAffect sideAffect;
    public int inventoryPosition = -1;

    // Constructor to create from ItemConfig
    public SerializableItemConfig(ItemConfig item)
    {
        if (item == null) return;

        this.itemName = item.itemName;
        this.iconPath = "Sprites/Items/" + item.icon.name + ".png";
        this.power = item.power;
        this.actionType = item.actionType;
        this.sideAffect = item.sideAffect;
        this.inventoryPosition = item.inventoryPosition;
    }

    // Method to convert back to ItemConfig
    public ItemConfig ToItemConfig()
    {
        var item = ScriptableObject.CreateInstance<ItemConfig>();
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