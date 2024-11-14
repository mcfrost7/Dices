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
        [SerializeField] private string typeName;
        [SerializeField] private Sprite sprite;
        [SerializeField] private int level;
        [SerializeField] private DiceConfig dice;

        public string TypeName { get => typeName; set => typeName = value; }
        public Sprite Sprite { get => sprite; set => sprite = value; }
        public int Level { get => level; set => level = value; }
        public DiceConfig Dice { get => dice; set => dice = value; }
    }
}

