using System;
using UnityEngine;

[Serializable]
public class SerializableResourceData
{
    [SerializeField] public string ConfigID;
    [SerializeField] public string Name;
    [SerializeField] public int Count;

    public SerializableResourceData() { }

    public SerializableResourceData(ResourceData resourceData)
    {
        if (resourceData.Config != null)
        {
            ConfigID = resourceData.Config._id;
            Name = resourceData.Config.name;
        }
        Count = resourceData.Count;
    }
}

[Serializable]
public class ResourceData
{
    [SerializeField] private ResourceConfig _config;
    [SerializeField] private int _count;
    public ResourceConfig Config { get => _config; set => _config = value; }
    public int Count { get => _count; set => _count = value; }

    public SerializableResourceData ToSerializable()
    {
        return new SerializableResourceData(this);
    }

    public ResourceData(ResourceConfig config, int count)
    {
        _config = config;
        _count = count;
    }

    public ResourceData() { }
}