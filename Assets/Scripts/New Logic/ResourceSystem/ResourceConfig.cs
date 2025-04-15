using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewResource", menuName = "Configs/NewResource")]
[Serializable]
public class ResourceConfig : ScriptableObject
{
    [SerializeField] private string resourceName; // Название ресурса
    [SerializeField] private Sprite icon;        // Иконка ресурса
    [SerializeField] private ResourcesType resourcesType; // Тип ресурса
    public string _id;
    public string ResourceName { get => resourceName; set => resourceName = value; }
    public Sprite Icon { get => icon; set => icon = value; }
    public ResourcesType ResourcesType { get => resourcesType; set => resourcesType = value; }

    public void Initialize(string name, Sprite resourceIcon, ResourcesType type)
    {
        resourceName = name;
        icon = resourceIcon;
        resourcesType = type;
    }
    private void OnValidate()
    {
        _id = this.name;
    }

}
