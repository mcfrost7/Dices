using UnityEngine;

public interface IEventHandler
{
    void HandleEvent(NewTileConfig config);
}
public class BattleEventHandler : IEventHandler
{
    public void HandleEvent(NewTileConfig config)
    {
        GlobalWindowController.Instance.ShowBattle();
        BattleController.Instance.InitializeBattle(config, false);
    }
}

public class RouletteEventHandler : IEventHandler
{
    private RouletteScreen _roulette;
    private GlobalWindowController _uiController;

    public RouletteEventHandler(RouletteScreen roulette, GlobalWindowController uiController)
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
        if (config.lootSettings.reward != null)
        { 
            RewardMNG.Instance.CalculateReward(config.lootSettings.reward);
            GameWindowController.Instance.SetupResourceInfo(config.lootSettings.reward);
            GameWindowController.Instance.CallPanel(1);
        }
    }
}


public class CampEventHandler : IEventHandler
{
    public void HandleEvent(NewTileConfig config)
    {
        UnitsPanelUI.Instance.AddButtonVisibility();
        CampPanel.Instance.SetupInfo(config);
        GameWindowController.Instance.SetupCampfireTileInfo(config);
        GameWindowController.Instance.CallPanel(1);
    }
}
public class BossEventHandler : IEventHandler
{
    public void HandleEvent(NewTileConfig config)
    {
        GlobalWindowController.Instance.ShowBattle();
        BattleController.Instance.InitializeBattle(config, true);
    }
}