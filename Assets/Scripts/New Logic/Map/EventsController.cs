using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EventsController : MonoBehaviour
{
    [SerializeField] private RouletteScreen _roulette;
    [SerializeField] private UIController _uiController;

    private Dictionary<TileType, IEventHandler> _eventHandlers;

    private void Awake()
    {
        InitializeEventHandlers();
    }

    private void InitializeEventHandlers()
    {
        _eventHandlers = new Dictionary<TileType, IEventHandler>
        {
            { TileType.BattleTile, new BattleEventHandler() },
            { TileType.CampTile, new CampEventHandler() },
            { TileType.BossTile, new BossEventHandler() },
            { TileType.RouleteTile, new RouletteEventHandler(_roulette, _uiController) },
            { TileType.LootTile, new ResourceEventHandler() }
        };
    }

    public void HandleEvent(TileType tileType, NewTileConfig config)
    {
        if (_eventHandlers.TryGetValue(tileType, out var handler))
        {
            handler.HandleEvent(config);
        }
        else
        {
            Debug.LogWarning($"No handler found for tile type: {tileType}");
        }
    }
}
