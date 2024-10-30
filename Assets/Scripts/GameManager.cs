using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject BattleManager;
    public GameObject MapManager;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Проверка нажатия левой кнопки мыши
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile != null && tile.isWalkable == true && tile.isPassed == false)
                {
                    MapManager mapManager = MapManager.GetComponent<MapManager>();
                    mapManager.UpdateTiles(tile);
                    switch (tile)
                    {
                        case BattleTile battleTile:
                            MapManager.SetActive(false);
                            BattleManager.SetActive(true);
                            break;
                        default:
                            Debug.LogWarning("Unknown tile type");
                            break;
                    }
                }
            }
        }
        if (BattleManager.activeSelf == false && MapManager.activeSelf == false)
        {
            MapManager.SetActive(true);
        }

    }
}
