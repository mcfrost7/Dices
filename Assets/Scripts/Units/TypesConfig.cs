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
        [SerializeField] private UnitSide unit_side;
        [SerializeField] private Sprite sprite_type;
        [SerializeField] private Sprite sprite_level;
        [SerializeField] private int level;
        [SerializeField] private DiceConfig dice;

        public string TypeName { get => typeName; set => typeName = value; }
        public Sprite Sprite_type { get => sprite_type; set => sprite_type = value; }
        public int Level { get => level; set => level = value; }
        public DiceConfig Dice { get => dice; set => dice = value; }
        public UnitSide Unit_side { get => unit_side; set => unit_side = value; }
        public Sprite Sprite_level { get => sprite_level; set => sprite_level = value; }
    }
}

