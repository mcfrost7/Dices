using UnityEngine;
using static UnityEditor.PlayerSettings.Switch;

public class Tile : MonoBehaviour
{
    public int type;
    public string tileName;
    public bool isWalkable;

    public void Initialize(int type, string name, bool isWalkable)
    {
        this.type = type;
        this.tileName = name;
        this.isWalkable = isWalkable;
    }

    public virtual void OnTileClicked()
    {

    }
}

public class BattleTile : Tile
{
    public override void OnTileClicked()
    {
        Debug.Log("бИТВА");

    }
}

public class LootTile : Tile
{
    public string lootType;

    public void Initialize(int type, string name, bool isWalkable, string lootType)
    {
        base.Initialize(type, name, isWalkable);
        this.lootType = lootType;
    }

    public override void OnTileClicked()
    {
        Debug.Log("Лут собран: " + lootType);
    }
}

public class CampfireTile : Tile
{
    public override void OnTileClicked()
    {
        Debug.Log("Отдых у костра!");
    }
}