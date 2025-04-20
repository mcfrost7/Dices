using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class RewardMNG : MonoBehaviour
{
    [SerializeField] private float _dropChance;
   public static RewardMNG Instance { get; private set; }
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
    
    public void CalculateReward(SerializableRewardConfig reward)
    {
        ResourceReward(reward.resources);
        ExpReward(reward.expAmount);
        ItemReward(reward);
    }

    private void ResourceReward(List<ResourceData> resource)
    {
        if (resource.Count > 0 && resource != null)
        {
            ResourcesMNG.Instance.AddResources(resource);
        }
    }
    private void ExpReward(int exp)
    {
        foreach (var unit in GameDataMNG.Instance.PlayerData.PlayerUnits)
        {
            if (unit == null) continue;

            int maxExp = unit._level * 10;
            unit._current_exp = Mathf.Min(unit._current_exp + exp, maxExp);
        }
    }
    private void ItemReward(SerializableRewardConfig reward)
    {
        if (reward == null) return;

        // Выбираем случайный предмет в момент получения награды
        ItemInstance item = reward.GetRandomItem();

        if (item != null && item.Config != null)
        {
            if (Random.Range(0f, 100f) <= _dropChance)
            {
                ItemMNG.Instance.AddItem(item);
            }
        }
    }

}
