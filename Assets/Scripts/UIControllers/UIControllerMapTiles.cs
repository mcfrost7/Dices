using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControllerMapTiles : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites_for_tileTypes;
    public Sprite[] Sprites_for_tileTypes { get => sprites_for_tileTypes; set => sprites_for_tileTypes = value; }

    public Sprite GetSpriteByType(TileType tileType)
    {
        foreach (var tile_sprite in Sprites_for_tileTypes )
        {
            if (tile_sprite.name == "battle_tile" && tileType == TileType.BattleTile)
                return tile_sprite;
            if (tile_sprite.name == "campfire_tile" && tileType == TileType.CampfireTile)
                return tile_sprite;
            if (tile_sprite.name == "loot_tile" && tileType == TileType.LootTile)
                return tile_sprite;
        }
        return null;
    }
}
