using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public void AddAllReward(RewardConfig reward)
    {
        //ResourceReward(reward.resource);
        ExperienceReward(reward.expAmount);
    }

    private void ResourceReward(int amount)
    {
        foreach (var resource in GameManager.Instance.Player.Resources)
        {
            if (resource.ResourcesType == ResourcesType.SignalTransmitter)
            {
                resource.AddAmount(amount);
            }
        }
    }

    private void ExperienceReward(int amount)
    {
        foreach (var unitStats in GameManager.Instance.Player.Units)
        {
            unitStats.AddExperience(amount);
        }
    }

    private void ItemReward(ItemConfig item)
    {
        GameManager.Instance.Player.Items.Add(item);
    }
}