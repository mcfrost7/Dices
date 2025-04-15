using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceControllerUI : MonoBehaviour
{
    public static ResourceControllerUI Instance { get; private set; }

    [SerializeField] private GameObject _spawnPanel;
    [SerializeField] private ResourcesPrefabSetup _prefab;
    [SerializeField] private List<ResourceConfig> _resources;

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

    public void CreateResourcesTopPanel()
    {
        ClearPanel();
        foreach (ResourceConfig resource in _resources)
        {
            ResourceData playerResource = GameDataMNG.Instance.PlayerData.ResourcesData.Find(r => r.Config == resource);
            int resourceCount = playerResource != null ? playerResource.Count : 0;

            ResourcesPrefabSetup resourceOnPanel = Instantiate(_prefab, _spawnPanel.transform);
            resourceOnPanel.Setup(resource.Icon, resource.ResourceName, resourceCount);
        }
    }

    public void UpdateResourcesTopPanel()
    {
        Dictionary<string, int> resourceAmounts = new Dictionary<string, int>();
        foreach (var resource in ResourcesMNG.Instance.Resources)
        {
            resourceAmounts[resource.Config.ResourceName] = resource.Count;
        }

        foreach (Transform child in _spawnPanel.transform)
        {
            ResourcesPrefabSetup resourceUI = child.GetComponent<ResourcesPrefabSetup>();
            if (resourceUI == null) continue;

            // Get name from the TextMeshProUGUI component
            string resourceName = resourceUI.Name.text;

            // Find matching resource by name
            if (resourceAmounts.TryGetValue(resourceName, out int count))
            {
                resourceUI.UpdateAmount(count);
            }
            else
            {
                resourceUI.UpdateAmount(0);
            }
        }
    }


    private void ClearPanel()
    {
        foreach (Transform child in _spawnPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }


}
