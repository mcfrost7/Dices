using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    private static BattleManager instance;

    public static BattleManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<BattleManager>();
            }
            return instance;
        }
    }

    public GameObject Action_manager { get => ActionManager; set => ActionManager = value; }
    public GameObject BattleUI { get => battleUI; set => battleUI = value; }
    public GameObject ActionManager { get => actionManager; set => actionManager = value; }
    public GameObject EnemyManager { get => enemyManager; set => enemyManager = value; }
    public GameObject DrawBattleManager { get => uiControllerBattleManager; set => uiControllerBattleManager = value; }
    public float DiceRollDelay { get => diceRollDelay; set => diceRollDelay = value; }
    public float TimeToWait { get => timeToWait; set => timeToWait = value; }
    public Button Button_reroll { get => button_reroll; set => button_reroll = value; }
    public Button Button_end { get => button_end; set => button_end = value; }
    public TextMeshProUGUI Text_end_reroll { get => text_end_reroll; set => text_end_reroll = value; }
    public TextMeshProUGUI Text_end_turn { get => text_end_turn; set => text_end_turn = value; }
    public int UsedUnits { get => usedUnits; set => usedUnits = value; }
    public BattlePhase CurrentPhase { get => currentPhase; set => currentPhase = value; }
    public List<GameObject> UnitList { get => unitList; set => unitList = value; }
    public List<GameObject> EnemyList { get => enemyList; set => enemyList = value; }
    public GameObject TeamManager { get => teamManager; set => teamManager = value; }
    public UIControllerBattle UiControllerBattle { get => uiControllerBattle; set => uiControllerBattle = value; }

    [SerializeField] private Camera mainCamera; // Основная камера
    [SerializeField] private GameObject battleUI; // UI для битвы
    [SerializeField] private GameObject actionManager; //совершение действий битвы
    [SerializeField] private GameObject teamManager; // Менеджер команды
    [SerializeField] private GameObject enemyManager; // Менеджер врагов
    [SerializeField] private GameObject uiControllerBattleManager; // Менеджер отрисовки

    [SerializeField] private float diceRollDelay = 0.05f; // Задержка для анимации кубика
    [SerializeField] private float timeToWait = 3f;

    [SerializeField] private Button button_reroll = null;
    [SerializeField] private Button button_end = null;
    [SerializeField] private TextMeshProUGUI text_end_reroll = null;
    [SerializeField] private TextMeshProUGUI text_end_turn = null;

    private int usedUnits = 0;
    private BattlePhase currentPhase = BattlePhase.BotRollsDice;
    private List<GameObject> unitList = new();
    private List<GameObject> enemyList = new();
    private UIControllerBattle uiControllerBattle = null;


    private void Awake()
    {
        gameObject.SetActive(false);
        uiControllerBattle = uiControllerBattleManager.GetComponent<UIControllerBattle>();
    }

    private void OnEnable()
    {
        ResetState();
        ResetUI();
        EnableBattleSystem(true);
    }



    private void ResetState()
    {
        // Очистка списков юнитов
        UnitList.Clear();
        EnemyList.Clear();


        // Сброс значений переменных для фаз боя и роллов
        CurrentPhase = BattlePhase.BotRollsDice;
        UiControllerBattle.SumOfReroll = 0;
        UiControllerBattle.CurrentRolls = 0;
        UsedUnits = 0;
        GameManager.Instance.Status = BattleStatus.None;
    }

    private void ResetUI()
    {
        // Сброс текста для отображения роллов и кнопок
        if (Text_end_reroll != null) Text_end_reroll.enabled = true;
        if (Text_end_turn != null) Text_end_turn.enabled = false;

        if (Button_reroll != null) Button_reroll.interactable = true;
        if (Button_end != null) Button_end.interactable = false;

        // Очистка всех панелей, если есть дочерние элементы
        ClearPanel(BattleUI.transform.Find("Units"));
        ClearPanel(BattleUI.transform.Find("Enemies"));
    }

    private void ClearPanel(Transform panel)
    {
        if (panel == null) return;

        foreach (Transform child in panel)
        {
            Destroy(child.gameObject);
        }
    }
    private void OnDisable()
    {
        EnableBattleSystem(false);
        GameManager.Instance.GoFromBattleToMap();
    }

    private void EnableBattleSystem(bool enable)
    {
        CameraMovement cameraMovement = FindObjectOfType<CameraMovement>();
        if (cameraMovement != null)
        {
            cameraMovement.SetCameraMovementEnabled(!enable);
        }

        if (BattleUI != null)
        {
            BattleUI.SetActive(enable);
        }

        if (enable)
        {
            EnemyManager enemyManagerTemp = EnemyManager.GetComponent<EnemyManager>();
            enemyManagerTemp.CreateEnemies();
            UiControllerBattle.DrawUnits();
            UiControllerBattle.DrawEnemies();

            NextPhase();
        }
    }

 
    public void RerollDice(bool isPlayer)
    {
        StartCoroutine(RollDiceCoroutine(isPlayer));
    }

    private IEnumerator RollDiceCoroutine(bool isPlayer)
    {
        GameManager gameManager = GameManager.Instance;

        // Заморозка интерфейса
        yield return StartCoroutine(SetInteractableCoroutine(true));

        Transform panel = isPlayer ? UiControllerBattle.UnitsPanel : UiControllerBattle.EnemiesPanel;
        List<GameObject> units = isPlayer ? UnitList : EnemyList;

        if (panel != null)
        {
            List<Coroutine> diceCoroutines = new List<Coroutine>();

            for (int i = 0; i < units.Count; i++)
            {
                // Проверяем, что индекс не выходит за пределы количества дочерних элементов
                if (i >= panel.childCount)
                {
                    Debug.LogWarning("Количество юнитов превышает количество дочерних объектов в панели.");
                    break;
                }

                Transform unitTransform = panel.GetChild(i);
                Transform diceSpriteTransform = unitTransform.Find("Dice");
                Image diceImage = diceSpriteTransform?.GetComponent<Image>();

                if (diceImage != null && (isPlayer ? units[i].GetComponent<Unit>().UnitStats.IsClickable : true)) 
                {
                    if (units[i] != null)
                        diceCoroutines.Add(StartCoroutine(RollDiceForUnit(i, diceImage, units[i].GetComponent<Unit>())));
                }
            }

            foreach (var coroutine in diceCoroutines)
            {
                yield return coroutine;
            }
        }
        yield return StartCoroutine(SetInteractableCoroutine(false));
    }

    private IEnumerator RollDiceForUnit(int unitIndex, Image diceImage, Unit unit)
    {
        int randomDiceSide = 0;
        DiceConfig diceConfig = unit.UnitStats.Type.Dice; // Получаем конфиг дайса юнита

        if (diceConfig == null || diceConfig.ActionSprites.Length == 0)
        {
            Debug.LogWarning("DiceConfig or actionSprites is missing for unit.");
            yield break; // Прекращаем выполнение, если конфиг или спрайты отсутствуют
        }

        for (int j = 0; j <= 20; j++)
        {
            // Генерируем случайный индекс для выбора стороны дайса
            randomDiceSide = Random.Range(0, diceConfig.ActionSprites.Length);
            diceImage.sprite = diceConfig.ActionSprites[randomDiceSide];
            yield return new WaitForSeconds(DiceRollDelay);
        }

        diceImage.sprite = diceConfig.ActionSprites[randomDiceSide];
        DiceConfig.DiceAction diceAction = diceConfig.DiceActions[randomDiceSide];
        unit.UnitStats.Current_dice_side = diceAction;
        Debug.Log($"Final dice side for {unit.UnitStats.Type.TypeName} - {unitIndex} : {randomDiceSide}");
    }

    private void NextPhase()
    {
        StartCoroutine(NextPhaseCoroutine());
    }

    private IEnumerator NextPhaseCoroutine()
    {
        // Проверяем, живы ли команды перед каждой фазой
        if (!IsAnyTeamAlive())
        {
            yield return new WaitForSeconds(TimeToWait);
            EndBattle();
            yield break; // Прерываем выполнение корутины, если бой завершён
        }
        switch (CurrentPhase)
        {
            case BattlePhase.BotRollsDice:
                UiControllerBattle.RerollsCount();
                UiControllerBattle.CurrentRolls = UiControllerBattle.SumOfReroll;
                UiControllerBattle.DrawRerolls();
                FreezeUIButtons(true);
                BotRollDiceAndSelectTarget();
                break;

            case BattlePhase.PlayerRollsDice:
                RerollDice(true);
                FreezeUIButtons(false);
                yield return new WaitForSeconds(TimeToWait);
                break;
            case BattlePhase.PlayerAction:

                break;
            case BattlePhase.BotAction:
                FreezeUIButtons(true);
                yield return new WaitForSeconds(TimeToWait);
                BotTakeAction();
                break;
        }
    }

    public void FreezeUIButtons(bool enabled)
    {
        Button_reroll.interactable = !enabled;
        Button_end.interactable = !enabled;
    }
    private void BotRollDiceAndSelectTarget()
    {
        RerollDice(false);
        EnemyManager enemyManagerLocal  = EnemyManager.GetComponent<EnemyManager>();
        enemyManagerLocal.FindTarget(unitList, enemyList);
        CurrentPhase = BattlePhase.PlayerRollsDice;
        NextPhase();
    }

    public void PlayerRollDice()
    {
        if (CurrentPhase == BattlePhase.PlayerRollsDice)
        {
            UiControllerBattle.DrawRerolls();
            RerollDice(true);
            UiControllerBattle.CurrentRolls--;
            UiControllerBattle.DrawRerolls();
            if (UiControllerBattle.CurrentRolls <= 0)
            {
                EndButonAction();
            }
        }
    }

    public void EndButonAction()
    {
        if (CurrentPhase == BattlePhase.PlayerRollsDice)
        {
            Button_reroll.interactable = false;
            Text_end_reroll.enabled = false;
            Text_end_turn.enabled = true;
            Button_end.interactable = true;
            CurrentPhase = BattlePhase.PlayerAction;
            NextPhase();
        }
        else if (CurrentPhase == BattlePhase.PlayerAction)
        {
            Action_manager.GetComponent<ActionManager>().EndTurn();
            Button_reroll.interactable = true;
            Text_end_reroll.enabled = true;
            Text_end_turn.enabled = false;
            UsedUnits = 0;
            foreach (var unit in unitList)
            {
                unit.GetComponent<Unit>().UnitStats.IsClickable = true;
            }
            CurrentPhase = BattlePhase.BotAction;
            NextPhase();
        }
    }


    public void PerformAction(Unit unit)
    {
        if (CurrentPhase == BattlePhase.PlayerAction)
        {
            Action_manager.GetComponent<ActionManager>().SelectUnit(unit);
            uiControllerBattle.UpdateAllEnemiesStats();
            uiControllerBattle.UpdateAllUnitsStats();
            CheckUsedUnits();

        }
    }

    private void CheckUsedUnits()
    {
        foreach (GameObject unitObject in UnitList)
        {
            if (unitObject.GetComponent<Unit>().Can_act == false)
            {
                UsedUnits++;
            }
        }
        if (UsedUnits == UnitList.Count)
        {
            EndButonAction();
        }
        else
        {
            UsedUnits = 0;
        }
    }

    public void OnUnitDeath(Unit unit)
    {
        if (unit.UnitStats.IsPlayerUnit)
        {

            unitList.Remove(unit.gameObject); // Удаляем юнита
            Destroy(unit.gameObject);
        }
        else
        {
            EnemyList.Remove(unit.gameObject); // Удаляем юнита
            Destroy(unit.gameObject);
        }

        UiControllerBattle.UpdateAllUnitsStats();
        UiControllerBattle.UpdateAllEnemiesStats();

        // Проверка конца игры
        if (!IsAnyTeamAlive())
        {
            CurrentPhase = BattlePhase.BotAction;
            EndBattle(); // Игрок проиграл
        }
    }

    private void BotTakeAction()
    {
        if (currentPhase == BattlePhase.BotAction)
        {
            StartCoroutine(BotActionWithDelay());
        }
    }

    private IEnumerator BotActionWithDelay()
    {
        foreach (GameObject enemy in enemyList)
        {
            // Выполнение действия для текущего врага
            actionManager.GetComponent<ActionManager>().PerformAction(
                enemy.GetComponent<Unit>(),
                enemy.GetComponent<Unit>().UnitStats.Target
            );
            uiControllerBattle.UpdateAllEnemiesStats();
            uiControllerBattle.UpdateAllUnitsStats();
            // Задержка между действиями
            yield return new WaitForSeconds(0.7f); // Замените 1.0f на нужное время задержки в секундах
        }


        // Переход к следующей фазе
        CurrentPhase = BattlePhase.BotRollsDice;
        NextPhase();
    }


    private bool IsPlayerTeamAlive()
    {
        return unitList.Count > 0; // Проверяем, есть ли юниты у игрока
    }

    private bool IsEnemyTeamAlive()
    {
        return enemyList.Count > 0; // Проверяем, есть ли юниты у врага
    }

    private bool IsAnyTeamAlive()
    {
        return IsPlayerTeamAlive() && IsEnemyTeamAlive(); // Проверяем, живы ли обе команды
    }

    private void EndBattle()
    {
        if ((IsPlayerTeamAlive() == true) && (IsEnemyTeamAlive() == false))
        {
            GameManager.Instance.Player.Units.Clear();
            foreach (GameObject unitObj in unitList)
            {
                GameManager.Instance.Player.Units.Add(unitObj.GetComponent<Unit>().UnitStats);
            }
            GameManager.Instance.Status = BattleStatus.Win;
        }
        else
        {
            GameManager.Instance.Status = BattleStatus.Lose;
        }
        GameManager.Instance.EndBattleChecker();
    }


    private IEnumerator SetInteractableCoroutine(bool isFrozen)
    {
        // Получаем или создаем CanvasGroup на родительском объекте
        Transform parentTransform = BattleUI.transform;
        // Список элементов для заморозки
        Transform[] elementsToFreeze = new Transform[]
        {
        parentTransform.Find("Units"),
        parentTransform.Find("Reroll"),
        parentTransform.Find("End")
        };

        foreach (Transform element in elementsToFreeze)
        {
            if (element != null)
            {
                // Добавляем CanvasGroup компонент к элементу, если он отсутствует
                CanvasGroup elementCanvasGroup = element.GetComponent<CanvasGroup>();
                if (elementCanvasGroup == null)
                {
                    elementCanvasGroup = element.gameObject.AddComponent<CanvasGroup>();
                }

                // Устанавливаем состояние для элемента
                elementCanvasGroup.interactable = !isFrozen;
                elementCanvasGroup.blocksRaycasts = !isFrozen;
                elementCanvasGroup.alpha = isFrozen ? 0.5f : 1f;
            }
        }


        yield return null;
    }

}
