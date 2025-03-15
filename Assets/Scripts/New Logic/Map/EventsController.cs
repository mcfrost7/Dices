using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class EventsController : MonoBehaviour
{
    [SerializeField] private RouletteScreen _roulette;
    [SerializeField] private UIController _uiController;
    private Dictionary<TileType, System.Action<NewTileConfig>> _eventHandlers;

    private void Awake()
    {
        InitializeEventHandlers();
    }

    private void InitializeEventHandlers()
    {
        _eventHandlers = new Dictionary<TileType, System.Action<NewTileConfig>>
        {
            { TileType.BattleTile, SetupBattle },
            { TileType.CampTile, SetupCamp },
            { TileType.BossTile, SetupBossBattle },
            { TileType.RouleteTile, SetupRoulette },
            { TileType.LootTile, SetupResources }
        };
    }

    public void HandleEvent(TileType tileType, NewTileConfig config)
    {
        if (_eventHandlers.TryGetValue(tileType, out var handler))
        {
            handler(config);
        }
        else
        {
            Debug.LogWarning($"No handler found for tile type: {tileType}");
        }
    }

    public void SetupBattle(NewTileConfig config)
    {
    }

    public void SetupResources(NewTileConfig config)
    {
    }

    public void SetupRoulette(NewTileConfig config)
    {
        _roulette.SetupRoulette(config.rouleteSettings._configs);
        UIController.Instance.ShowRoulette();
    }

    public void SetupCamp(NewTileConfig config)
    {
    }

    public void SetupBossBattle(NewTileConfig config)
    {
    }
}