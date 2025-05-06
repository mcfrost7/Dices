using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileUIController : MonoBehaviour
{
    public static TileUIController Instance { get; private set; }

    [SerializeField] private Button _healButton;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeUIOnTileClick()
    {
        UpdateUIForTileType(GameDataMNG.Instance.CurrentTile.tileType);
    }

    private void UpdateUIForTileType(TileType tileType)
    {
        ChangeHealButtonAvailability(tileType);
        if (tileType == TileType.BattleTile || tileType == TileType.BossTile)
            ChangeTeamButtonAvailability();
    }

    private void ChangeHealButtonAvailability(TileType tileType)
    {
        _healButton.gameObject.SetActive(tileType == TileType.CampTile);
    }

    private void ChangeTeamButtonAvailability()
    {
        MenuMNG.Instance.SetInnactiveDownPanel();
    }


}
