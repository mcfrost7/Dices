using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RewardConfig
{
    public List<ResourceData> resource = null;
    public int expAmount = 0;
    public List<ItemConfig> items = null;
    [HideInInspector]public ItemInstance itemInstance = null;

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
