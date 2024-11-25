using UnityEngine;
using static UnityEditor.PlayerSettings.Switch;
public class Tile : MonoBehaviour
{
    private TileConfig.TileData tileData;
    private Sprite tile_sprite;

    public TileConfig.TileData TileData { get => tileData; set => tileData = value; }
    public Sprite Tile_sprite { get => tile_sprite; set => tile_sprite = value; }

    public void Initialize(TileConfig.TileData tileData)
    {
        this.TileData = SetFirstTile(tileData); 
        this.Tile_sprite = GameManager.Instance.MapManager1.GetComponent<UIControllerMapTiles>().GetSpriteByType(TileData.TileType);
    }

    public virtual void OnTileClicked()
    {
    }

    private TileConfig.TileData SetFirstTile(TileConfig.TileData tileData)
    {
        if (tileData != null)
        {
            if (tileData.Level == 1)
            {
                tileData.IsWalkable = true;
            }
            return tileData;
        }
        else
        {
            return tileData;
        }
    }
}


public class BattleTile : Tile
{
    public override void OnTileClicked()
    {
        GameManager.Instance.EnableManager(GameManager.Instance.MapManager1, false);
        GameManager.Instance.EnableManager(GameManager.Instance.BattleManager1, true);
    }
}


public class LootTile : Tile
{
    public Resource lootType;

    public void Initialize(TileConfig.TileData tileData, Resource lootType)
    {
        base.Initialize(tileData);
        this.lootType = lootType;
    }

    public override void OnTileClicked()
    {
        if (TileData.IsWalkable == true)
        {

            GameManager.Instance.BattleManager1.GetComponent<RewardManager>().ResourceReward();
            GameManager.Instance.MapManager1.GetComponent<TileManager>().UpdateTileData(this);
            GameManager.Instance.MenuManager1.GetComponent<UIControllerResources>().UpdateResources();
            GameManager.Instance.MenuManager1.GetComponent<UIControllerMenuInfoPanel>().ChooseInfo(TileType.LootTile,0,lootType);
            GameManager.Instance.MenuManager1.GetComponent<UIControllerMenuInfoPanel>().Main_panel.SetActive(true);
            GameManager.Instance.EnableManager(GameManager.Instance.MapManager1, false);
            GameManager.Instance.EnableManager(GameManager.Instance.MapManager1, true);
        }
    }
}

public class CampfireTile : Tile
{
    public override void OnTileClicked()
    {
        int heal = 3;

        if (TileData.IsWalkable == true)
        {
            foreach (var unitStats in GameManager.Instance.Player.Units)
            {
                unitStats.UpdateHealth(heal);
            }
            GameManager.Instance.MenuManager1.GetComponent<UIControllerMenuInfoPanel>().ChooseInfo(TileType.CampfireTile, heal);
            GameManager.Instance.MenuManager1.GetComponent<UIControllerMenuInfoPanel>().Main_panel.SetActive(true);
            GameManager.Instance.MenuManager1.GetComponent<UIControllerResources>().UpdateResources();
            GameManager.Instance.MapManager1.GetComponent<TileManager>().UpdateTileData(this);
            GameManager.Instance.EnableManager(GameManager.Instance.MapManager1, false);
            GameManager.Instance.EnableManager(GameManager.Instance.MapManager1, true);
        }
    }
}