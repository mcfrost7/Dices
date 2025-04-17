using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RewardConfig
{
    public List<ResourceData> resource = null;
    public int expAmount = 0;
    public ItemConfig item = null;
    [HideInInspector]public ItemInstance itemInstance = null;

    public ItemInstance GetItem()
    {
        if (item != null)
            return new ItemInstance(item);
        else 
            return null;
    }
}
