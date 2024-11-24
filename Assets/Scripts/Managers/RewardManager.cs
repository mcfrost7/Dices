using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    private void ResourceReward()
    {
        foreach (var resource in GameManager.Instance.Player.Resources)
        {
            if (resource.ResourcesType == ResourcesType.SignalTransmitter )
            {
                resource.AddAmount(1);
            }
        }

    }

    private void ExperienceReward()
    {
        foreach (var unitStats in GameManager.Instance.Player.Units)
        {
            unitStats.AddExperience(1);
        }
    }

    public void AddReward()
    {
        ResourceReward();
        ExperienceReward();
    }

}
