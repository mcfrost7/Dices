using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TypesConfig", menuName = "ScriptableObjects/TypesConfig")]
public class TypesInfo : ScriptableObject
{
    public Type[] types;

    [System.Serializable]
    public class Type
    { 
        public string typeName;
        public Sprite sprite;
        public int level;
        public DiceConfig dice;
    }
}

