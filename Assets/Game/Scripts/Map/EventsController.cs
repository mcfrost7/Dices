using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EventsController : MonoBehaviour
{
    [SerializeField] private RouletteScreen _roulette;
    [SerializeField] private GlobalWindowController _uiController;

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
            { TileType.RouletteTile, new RouletteEventHandler(_roulette, _uiController) },
            { TileType.LootTile, new ResourceEventHandler() }
        };
    }

    public void HandleEvent(CanvasMapGenerator.MapNode mapNode)
    {
        if (_eventHandlers.TryGetValue(mapNode.tileType, out var handler))
        {
            handler.HandleEvent(mapNode.tileConfig);
        }

    }
}
