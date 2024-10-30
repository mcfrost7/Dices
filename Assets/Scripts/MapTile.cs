using UnityEngine;
using static UnityEditor.PlayerSettings.Switch;
public class Tile : MonoBehaviour
{
    public int level;
    public string tileName;
    public bool isWalkable;
    public bool isPassed;
    private Canvas globalCanvas;
    private Canvas battleCanvas;
    private GameObject manager;

    public Canvas GlobalCanvas
    {
        get { return globalCanvas; }
        set { globalCanvas = value; }
    }

    public Canvas BattleCanvas
    {
        get { return battleCanvas; }
        set { battleCanvas = value; }
    }

    public void Initialize(int level, string name, bool isWalkable, bool isPassed )
    {
        this.level = level;
        this.tileName = name;
        this.isWalkable = isWalkable;
        this.isPassed = isPassed; 
    }

    public virtual void OnTileClicked()
    {
    }
}


public class BattleTile : Tile
{
    public override void OnTileClicked()
    {

    }
}


public class LootTile : Tile
{
    public string lootType;

    public void Initialize(int type, string name, bool isWalkable, string lootType, bool isPassed)
    {
        base.Initialize(type, name, isWalkable, isPassed);
        this.lootType = lootType;
    }

    public override void OnTileClicked()
    {
        if (isWalkable == true)
        {
            Debug.Log("Лут собран: " + lootType);
        }
    }
}

public class CampfireTile : Tile
{
    public override void OnTileClicked()
    {
        if (isWalkable == true)
        {
            Debug.Log("Отдых у костра!");
        }
    }
}