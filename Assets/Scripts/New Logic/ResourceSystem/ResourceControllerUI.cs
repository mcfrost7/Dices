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
        foreach (ResourceConfig resource in _resources)
        {
            // Найти соответствующий ресурс в PlayerData
            ResourceData playerResource = GameDataMNG.Instance.PlayerData.Resources.Find(r => r.Config == resource);

            // Получить количество ресурса (если он есть у игрока, иначе 0)
            int resourceCount = playerResource != null ? playerResource.Count : 0;

            // Создать UI-элемент для ресурса
            ResourcesPrefabSetup resourceOnPanel = Instantiate(_prefab, _spawnPanel.transform);

            // Настроить отображение ресурса
            resourceOnPanel.Setup(resource.Icon, resource.ResourceName, resourceCount);
        }
    }

    public void UpdateResourcesTopPanel()
    {
        foreach (Transform child in _spawnPanel.transform)
        {
            ResourcesPrefabSetup resourceUI = child.GetComponent<ResourcesPrefabSetup>();
            if (resourceUI == null) continue;

            // Найти соответствующий ресурс в PlayerData
            ResourceData playerResource = GameDataMNG.Instance.PlayerData.Resources.Find(r => r.Config.ResourceName == resourceUI.name);

            // Получить актуальное количество ресурса
            int resourceCount = playerResource != null ? playerResource.Count : 0;

            // Обновить отображение ресурса
            resourceUI.UpdateAmount(resourceCount);
        }
    }


}
