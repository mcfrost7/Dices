using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerResources : MonoBehaviour
{
    [SerializeField] private GameObject resourse_prefab; // Префаб ресурса
    [SerializeField] private GameObject panel_main;      // Панель, в которую добавляются ресурсы
    private List<GameObject> resourceInstances = new List<GameObject>(); // Список для хранения экземпляров UI ресурсов

    // Создание нового ресурса
    public void CreateResource(string name, Sprite sprite, int amount)
    {
        if (resourse_prefab == null || panel_main == null)
        {
            Debug.LogError("Префаб или панель не назначены в инспекторе.");
            return;
        }

        // Создаём экземпляр префаба
        GameObject resourceInstance = Instantiate(resourse_prefab);
        // Устанавливаем родителя (panel_main)
        resourceInstance.transform.SetParent(panel_main.transform, false);
        ResourcesPrefabSetup resourceSetup = resourceInstance.GetComponent<ResourcesPrefabSetup>();

        if (resourceSetup != null)
        {
            resourceSetup.Setup(sprite, name, amount);
            resourceInstances.Add(resourceInstance); // Добавляем созданный экземпляр в список
        }
        else
        {
            Debug.LogError("Префаб ресурса не содержит компонент ResourcesTypeSetup.");
        }
    }

    // Метод для обновления всех ресурсов на UI
    public void UpdateResources()
    {
        // Получаем список всех ресурсов
        List<Resource> resources = GameManager.Instance.Player.Resources;

        if (resources == null || resources.Count == 0)
        {
            Debug.LogWarning("Нет ресурсов для отображения.");
            return;
        }

        // Для каждого ресурса обновляем его UI отображение
        for (int i = 0; i < resources.Count; i++)
        {
            Resource resource = resources[i];
            GameObject resourceInstance = resourceInstances[i];

            // Обновляем отображение ресурса, если оно существует
            if (resourceInstance != null)
            {
                ResourcesPrefabSetup resourceSetup = resourceInstance.GetComponent<ResourcesPrefabSetup>();
                if (resourceSetup != null)
                {
                    resourceSetup.Setup(resource.Icon, resource.Name, resource.Amount); // Обновляем UI
                }
            }
        }
    }

    // Метод для отображения всех ресурсов
    public void SpawnResources()
    {
        ClearResources();
        // Получаем список всех ресурсов
        List<Resource> resources = GameManager.Instance.Player.Resources;

        if (resources == null || resources.Count == 0)
        {
            Debug.LogWarning("Нет ресурсов для отображения.");
            return;
        }

        // Создаём UI элементы для каждого ресурса
        foreach (var resource in resources)
        {
            CreateResource(resource.Name, resource.Icon, resource.Amount);
        }
    }

    public void ClearResources()
    {

        foreach (var resourceInstance in resourceInstances)
        {
            if (resourceInstance != null)
            {
                Destroy(resourceInstance);
            }
        }

        // Очищаем список
        resourceInstances.Clear();
    }


    private void OnDisable()
    {
        GameManager.Instance.Player.Resources.Clear();
        ClearResources ();
    }
}
