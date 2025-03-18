using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileUIController : MonoBehaviour
{
    public static TileUIController Instance { get; private set; }

    [SerializeField] private Button _healButton;
    [SerializeField] private Button _teamButton;

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
        ChangeTeamAvailability();
    }

    private void ChangeHealButtonAvailability(TileType tileType)
    {
        _healButton.gameObject.SetActive(tileType == TileType.CampTile);
    }

    private void ChangeTeamAvailability()
    {
        bool isCanvasActive = GlobalWindowController.Instance.IsBattleActive();
        _teamButton.gameObject.SetActive(!isCanvasActive);
    }


}
