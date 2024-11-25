using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    [SerializeField] private int resource_amount;
    [SerializeField] private int exp_amount;

    public int Resource_amount { get => resource_amount; set => resource_amount = value; }
    public int Exp_amount { get => exp_amount; set => exp_amount = value; }

    public void ResourceReward()
    {
        foreach (var resource in GameManager.Instance.Player.Resources)
        {
            if (resource.ResourcesType == ResourcesType.SignalTransmitter )
            {
                resource.AddAmount(Resource_amount);
            }
        }

    }

    private void ExperienceReward()
    {
        foreach (var unitStats in GameManager.Instance.Player.Units)
        {
            unitStats.AddExperience(Exp_amount);
        }
    }

    public void AddReward()
    {
        ResourceReward();
        ExperienceReward();
    }

}
