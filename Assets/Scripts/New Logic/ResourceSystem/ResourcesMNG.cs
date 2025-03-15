using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesMNG : MonoBehaviour
{
    public static ResourcesMNG Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetupResources()
    {
        ResourceControllerUI.Instance.CreateResourcesTopPanel();
    }

    public void AddResources()
    {

    }

    public void RemoveResources() 
    {
        
    }

    public void AddExperience()
    {

    }

    public void AddItem()
    {

    }
}
