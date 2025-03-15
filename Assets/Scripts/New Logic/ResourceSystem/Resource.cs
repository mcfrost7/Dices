using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourceData 
{
    private ResourceConfig _config; 
    private int _count;

    public ResourceConfig Config { get => _config; set => _config = value; }
    public int Count { get => _count; set => _count = value; }
}
