using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourcesMNG : MonoBehaviour
{
    public static ResourcesMNG Instance { get; private set; }
    public List<ResourceData> Resources { get => _resources; set => _resources = value; }

    private List<ResourceData> _resources;

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
        _resources = new List<ResourceData>();
        ResourceControllerUI.Instance.CreateResourcesTopPanel();
    }

    public void LoadResources(PlayerData playerData)
    {
        if (playerData == null)
        {
            Debug.LogWarning("Cannot load resources: Player data is null");
            return;
        }

        // Ensure resources list exists
        if (playerData.ResourcesData != null)
        {
            _resources = playerData.ResourcesData;
        }
        else
        {
            _resources = new List<ResourceData>();
            playerData.ResourcesData = _resources;
        }

        ResourceControllerUI.Instance.UpdateResourcesTopPanel();
    }

    public void AddResources(List<ResourceData> newResources)
    {
        if (newResources == null || newResources.Count == 0)
            return;
        ResourceData existingResource;
        // Process each new resource
        foreach (ResourceData newResource in newResources)
        {
            if (newResource.Config == null || newResource.Count <= 0)
                continue;
            existingResource = _resources.Find(r => r.Config.ResourceName == newResource.Config.ResourceName);

            if (existingResource != null)
            {
                existingResource.Count += newResource.Count;
            }
            else
            {
                _resources.Add(new ResourceData(newResource.Config, newResource.Count));
            }
        }

        // Update UI
        ResourceControllerUI.Instance.UpdateResourcesTopPanel();
        SaveResource();
        existingResource = null;
    }

    private void SaveResource()
    {
        GameDataMNG.Instance.PlayerData.ResourcesData = Resources;
    }

    public bool TryConsumeResource(ResourcesType resourceType, int amount)
    {
        ResourceData resource = Resources.Find(r => r.Config.ResourcesType == resourceType);

        if (resource != null && resource.Count >= amount)
        {
            resource.Count -= amount; // Вычитаем ресурс
            ResourceControllerUI.Instance.UpdateResourcesTopPanel(); // Обновляем UI
            SaveResource(); // Сохраняем изменения
            ResourceControllerUI.Instance.UpdateResourcesTopPanel();
            return true;
        }
        ResourceControllerUI.Instance.UpdateResourcesTopPanel();
        Debug.LogWarning($"Недостаточно ресурса: {resourceType}");
        return false;
    }
}