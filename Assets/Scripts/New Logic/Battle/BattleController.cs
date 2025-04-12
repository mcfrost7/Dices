using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    private FSM _battleFsm;
    public static BattleController Instance { get; private set; }
    public List<NewUnitStats> PlayerUnits { get => playerUnits; set => playerUnits = value; }
    public List<NewUnitStats> EnemyUnits { get => enemyUnits; set => enemyUnits = value; }
    public List<BattleUnit> UnitsObj { get => unitsObj; set => unitsObj = value; }
    public List<BattleUnit> EnemiesObj { get => enemiesObj; set => enemiesObj = value; }

    private NewTileConfig _currentBattleConfig;
    private bool _isBossBattle;
    private List<NewUnitStats> playerUnits;
    private List<NewUnitStats> enemyUnits;
    private List<BattleUnit> unitsObj;
    private List<BattleUnit> enemiesObj;


    // References to important battle components
    [SerializeField] private Transform _playerUnitsContainer;
    [SerializeField] private Transform _enemyUnitsContainer;
    [SerializeField] private BattleUnit _unitprefab;
    [SerializeField] private BattleUnit _enemyprefab;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        _battleFsm?.Update();
    }

    // Called from BattleEventHandler or BossEventHandler
    public void InitializeBattle(NewTileConfig config, bool isBoss)
    {
        _currentBattleConfig = config;
        _isBossBattle = isBoss;
        SetupBattleUnits(config);
        InitializeBattleFSM();
    }

    private void SetupBattleUnits(NewTileConfig config)
    {
        ClearBattleField();
        SpawnPlayerUnits();
        SpawnEnemyUnits(config);
    }

    private void ClearBattleField()
    {
        foreach (Transform child in _playerUnitsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in _enemyUnitsContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void SpawnPlayerUnits()
    {
        PlayerUnits = TeamMNG.Instance.GetPlayerUnits();
        UnitsObj = new List<BattleUnit>();
        foreach (var unitData in PlayerUnits)
        {
            BattleUnit unitObj = Instantiate(_unitprefab, _playerUnitsContainer);
            unitObj.SetupUnit(unitData);
            UnitsObj.Add(unitObj);
        }
    }

    private void SpawnEnemyUnits(NewTileConfig config)
    {
        EnemyCreator creator = new EnemyCreator();
        EnemyUnits = creator.CreateEnemy(config);
        EnemiesObj = new List<BattleUnit>();
        foreach (var unitData in EnemyUnits)
        {
            BattleUnit unitObj = Instantiate(_enemyprefab, _enemyUnitsContainer);
            unitObj.SetupEnemy(unitData);
            EnemiesObj.Add(unitObj);
        }
    }

    private void InitializeBattleFSM()
    {
        _battleFsm = new FSM();

        // Create all states
        var intentionState = new FsmStateIntention(_battleFsm);
        var rollingState = new FsmStateRolling(_battleFsm);
        var rerollState = new FsmStateReroll(_battleFsm);
        var actionState = new FsmStateAction(_battleFsm);
        var botActionState = new FsmStateBotAction(_battleFsm);
        var winState = new FsmStateWin(_battleFsm);
        var loseState = new FsmStateLose(_battleFsm);

        // Add states to FSM
        _battleFsm.AddState(intentionState);
        _battleFsm.AddState(rollingState);
        _battleFsm.AddState(rerollState);
        _battleFsm.AddState(actionState);
        _battleFsm.AddState(botActionState);
        _battleFsm.AddState(winState);
        _battleFsm.AddState(loseState);

        // Set initial state
        _battleFsm.SetState<FsmStateIntention>();
    }
    public void OnBattleWin()
    {
        RewardConfig reward = null;
        if (_isBossBattle)
        {
            reward = _currentBattleConfig.bossSettings.reward;
        }
        {
            reward = _currentBattleConfig.battleSettings.reward;
        }
        BattleReward.Instance.CalculateReward(reward);
    }

    public void OnBattleLose()
    {
        GlobalWindowController.Instance.GoBack();
    }
}
