using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]private GameObject BattleManager;
    [SerializeField] private GameObject MapManager;
    [SerializeField] private GameObject MenuManager;
    [SerializeField] private GameObject TeamManager;
    [SerializeField] private Canvas Canvas;
    private Player player = new();
    private BattleStatus status = BattleStatus.None;
    private Tile currentTile = null;

    private bool IsNewGame;

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<GameManager>();
            }
            return instance;
        }
    }

    public GameObject BattleManager1 { get => BattleManager; set => BattleManager = value; }
    public GameObject MapManager1 { get => MapManager; set => MapManager = value; }
    public GameObject MenuManager1 { get => MenuManager; set => MenuManager = value; }
    public GameObject TeamManager1 { get => TeamManager; set => TeamManager = value; }
    public Canvas Canvas1 { get => Canvas; set => Canvas = value; }
    public Player Player { get => player; set => player = value; }
    public bool IsNewGame1 { get => IsNewGame; set => IsNewGame = value; }
    public BattleStatus Status { get => status; set => status = value; }

    public void EnableManager(GameObject manager, bool enable)
    {
        manager.SetActive(enable);
    }

    private void Awake()
    {
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        if (GetGameStatus() == true)
        {
            gameObject.GetComponent<GameStateManager>().StartNewGame();
        }
        else
        {
            gameObject.GetComponent<GameStateManager>().ContinueGame();
        }
        Canvas1.gameObject.SetActive(true);
        EnableManager(MapManager1, true);
    }
    private void OnDisable()
    {
        if (Canvas1 != null) Canvas1.gameObject.SetActive(false);
        EnableManager(MapManager1, false);
        EnableManager(BattleManager1, false);
        EnableManager(TeamManager1, false);
        EnableManager(MenuManager1 , false);
        EnableManager(MenuManager1, true);
    }

    public void BattleTileClick(Tile tile)
    {
        if (tile == null)
            return;
        if (tile.TileData.IsWalkable == true && tile.TileData.IsPassed == false)
        {
            tile.OnTileClicked();
            currentTile = tile;
        }

    }
    public void LootTileClick(Tile tile)
    {
        if (tile == null)
            return;
        if (tile.TileData.IsWalkable == true && tile.TileData.IsPassed == false)
        {
            tile.OnTileClicked();
            currentTile = tile;
        }

    }
    public void CampfireTileClick(Tile tile)
    {
        if (tile == null)
            return;
        if (tile.TileData.IsWalkable == true && tile.TileData.IsPassed == false)
        {
            tile.OnTileClicked();
            currentTile = tile;
        }
    }

    public void EndBattleChecker()
    {
        if (Status == BattleStatus.Win)
        {
            MapManager1.GetComponent<TileManager>().UpdateTileData(currentTile);
            BattleManager1.GetComponent<RewardManager>().AddReward();

        }
        MenuManager1.GetComponent<UIControllerNotification>().LoadInfoMenu();
    }

    public void BackFromTeamToMap()
    {
        EnableManager(MapManager1, true);
        EnableManager(TeamManager1, false);
    }

    public void GoFromMapToTeam()
    {
        EnableManager(MapManager1, false);
        EnableManager(TeamManager1, true);
    }
    public void GoFromBattleToMap()
    {
        EnableManager(BattleManager1, false);
        EnableManager(MapManager1, true);
    }

    public void SetGameStatus(bool status)
    {
        IsNewGame1 = status;
    }
    public bool GetGameStatus(){ return IsNewGame1; }

    public void SetActualData()
    {

    }

}
