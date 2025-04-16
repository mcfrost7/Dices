using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleReward : MonoBehaviour
{
   public static BattleReward Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    public void CalculateReward(RewardConfig reward)
    {
        ResourceReward(reward.resource);
        ExpReward(reward.expAmount);
        ItemReward(reward.itemInstance);
    }

    private void ResourceReward(List<ResourceData> resource)
    {
        if (resource.Count > 0)
        {
            ResourcesMNG.Instance.AddResources(resource);
        }
    }
    private void ExpReward(int exp)
    {
        foreach (var unit in BattleController.Instance.PlayerUnits)
        {
            if (unit == null) continue;

            int maxExp = unit._level * 10;
            unit._current_exp = Mathf.Min(unit._current_exp + exp, maxExp);
        }
    }
    private void ItemReward(ItemInstance item)
    {
        if (item == null) return;
        GameDataMNG.Instance.PlayerData.Items.Add(item);
    }
}
