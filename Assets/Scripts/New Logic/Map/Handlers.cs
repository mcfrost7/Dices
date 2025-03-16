using UnityEngine;

public interface IEventHandler
{
    void HandleEvent(NewTileConfig config);
}
public class BattleEventHandler : IEventHandler
{
    public void HandleEvent(NewTileConfig config)
    {
        Debug.Log("Handling Battle Event");
        // Логика битвы
    }
}

public class RouletteEventHandler : IEventHandler
{
    private RouletteScreen _roulette;
    private UIController _uiController;

    public RouletteEventHandler(RouletteScreen roulette, UIController uiController)
    {
        _roulette = roulette;
        _uiController = uiController;
    }

    public void HandleEvent(NewTileConfig config)
    {
        _roulette.SetupRoulette(config.rouleteSettings._configs);
        _uiController.ShowRoulette();
    }
}

public class ResourceEventHandler : IEventHandler
{
    public void HandleEvent(NewTileConfig config)
    {
        ResourcesMNG.Instance.AddResources(config.lootSettings.reward.resource);
    }
}


public class CampEventHandler : IEventHandler
{
    public void HandleEvent(NewTileConfig config)
    {
        UnitsPanelUI.Instance.CampSetup();
    }
}
public class BossEventHandler : IEventHandler
{
    public void HandleEvent(NewTileConfig config)
    {
    }
}