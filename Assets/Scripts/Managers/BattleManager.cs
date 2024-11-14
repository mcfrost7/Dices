using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    [SerializeField] private GameObject battleUI; // UI для битвы
    [SerializeField] private Camera mainCamera; // Основная камера
    [SerializeField] private GameObject mapManager; // Менеджер карты
    [SerializeField] private GameObject enemyManager; // Менеджер врагов
    [SerializeField] private GameObject unitPrefab; // Префаб юнита
    [SerializeField] private float diceRollDelay = 0.05f; // Задержка для анимации кубика
    [SerializeField] private float timeToWait = 3f;
    [SerializeField] private Button button_reroll = null;
    [SerializeField] private Button button_end = null;
    [SerializeField] private TextMeshProUGUI text_end_reroll = null;
    [SerializeField] private TextMeshProUGUI text_end_turn = null;
    private bool[] diceFrozen;
    private int clickedUnits = 0;
    private int sumOfReroll = 0, currentRolls = 0;
    private BattlePhase currentPhase = BattlePhase.BotRollsDice;
    private int[] finalSide = new int[6];
    private List<GameObject> unitList = new List<GameObject>();
    private List<GameObject> enemyList = new List<GameObject>();
    private GameObject current_clicked_unit_1 = null;
    private GameObject current_clicked_unit_2 = null;


    private void Awake()
    {
        gameObject.SetActive(false);
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
        unitList.Clear();
        enemyList.Clear();

        // Сброс текущих выбранных юнитов
        current_clicked_unit_1 = null;
        current_clicked_unit_2 = null;

        // Сброс значений переменных для фаз боя и роллов
        currentPhase = BattlePhase.BotRollsDice;
        clickedUnits = 0;
        sumOfReroll = 0;
        currentRolls = 0;

        // Сброс статуса кубиков
        diceFrozen = new bool[0];
    }

    private void ResetUI()
    {
        // Сброс текста для отображения роллов и кнопок
        if (text_end_reroll != null) text_end_reroll.enabled = true;
        if (text_end_turn != null) text_end_turn.enabled = false;

        if (button_reroll != null) button_reroll.interactable = true;
        if (button_end != null) button_end.interactable = false;

        // Очистка всех панелей, если есть дочерние элементы
        ClearPanel(battleUI.transform.Find("Units"));
        ClearPanel(battleUI.transform.Find("Enemies"));
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

        if (battleUI != null)
        {
            battleUI.SetActive(enable);
        }

        if (enable)
        {
            EnemyManager enemyManagerTemp = enemyManager.GetComponent<EnemyManager>();
            enemyManagerTemp.CreateEnemies();
            DrawUnits();
            DrawEnemies();
            NextPhase();
        }
    }

    private void SpawnEntities(GameObject entityPrefab, Transform panel, List<UnitStats> entities, bool isPlayerUnit)
    {
        if (panel == null || entityPrefab == null || entities == null) return;

        foreach (Transform child in panel)
        {
            Destroy(child.gameObject);  // Очистка перед спавном новых юнитов
        }

        for (int i = 0; i < entities.Count; i++)  // Добавление индекса в цикл
        {
            var entity = entities[i];
            var entityObject = Instantiate(entityPrefab, panel);
            Unit unit = entityObject.GetComponent<Unit>();
            unit.Init(entity);
            unit.UpdateUI();
            unit.UpdateImage();
            unit.UpdateDice(isPlayerUnit, i);
            unit.UpdateActionTrigger(isPlayerUnit, unit);
            unit.GetComponent<Unit>().Init(entity);
            if (isPlayerUnit)
            {
                unitList.Add(entityObject);
            }
            else
            {
                enemyList.Add(entityObject);
            }
            entityObject.transform.localScale = Vector3.one;
        }
    }

    public void DrawUnits()
    {
        GameManager gameManager = GameManager.Instance;
        Transform unitsPanel = battleUI.transform.Find("Units");
        diceFrozen = new bool[gameManager.player.units.Count];
        for (int i = 0; i < gameManager.player.units.Count; i++)
        {
            diceFrozen[i] = false;
        }
        SpawnEntities(unitPrefab, unitsPanel, gameManager.player.units,true);
    }

    public void DrawEnemies()
    {
        EnemyManager enemyManagerTemp = enemyManager.GetComponent<EnemyManager>();
        Transform enemiesPanel = battleUI.transform.Find("Enemies");
        SpawnEntities(enemyManagerTemp.EnemyPrefab, enemiesPanel, enemyManagerTemp.Enemies, false);
    }
    public void UpdateAllUnitsStats()
    {
        GameManager gameManager = GameManager.Instance;
        Transform unitsPanel = battleUI.transform.Find("Units");
        UpdateEntitiesStats(unitList, unitsPanel);
    }

    public void UpdateAllEnemiesStats()
    {
        EnemyManager enemyManagerTemp = enemyManager.GetComponent<EnemyManager>();
        Transform enemiesPanel = battleUI.transform.Find("Enemies");
        UpdateEntitiesStats(enemyList, enemiesPanel);
    }
    private void UpdateEntitiesStats(List<GameObject> entities, Transform panel)
    {
        // Проверяем, что переданы данные
        if (entities == null || panel == null) return;

        for (int i = 0; i < entities.Count; i++)
        {
            var entity = entities[i];

            entity.GetComponent<Unit>().UpdateUI();
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

        Transform panel = isPlayer ? battleUI.transform.Find("Units") : battleUI.transform.Find("Enemies");
        List<GameObject> units = isPlayer ? unitList : enemyList;

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

                if (diceImage != null && (isPlayer ? !diceFrozen[i] : true))
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
            yield return new WaitForSeconds(diceRollDelay);
        }

        finalSide[unitIndex] = randomDiceSide;
        DiceConfig.DiceAction diceAction = diceConfig.DiceActions[finalSide[unitIndex]];
        diceConfig.Current_dice_side = diceAction.actionType;
        Debug.Log($"Final dice side for {unit.UnitStats.Type.TypeName} - {unitIndex} : {finalSide[unitIndex]}");
    }


    public void OnDiceClick(int i)
    {
        diceFrozen[i] = !diceFrozen[i];
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
            yield return new WaitForSeconds(timeToWait);
            EndBattle();
            yield break; // Прерываем выполнение корутины, если бой завершён
        }
        switch (currentPhase)
        {
            case BattlePhase.BotRollsDice:
                RerollsCount();
                currentRolls = sumOfReroll;
                DrawRerolls();
                FreezeUIButtons(true);
                BotRollDiceAndSelectTarget();
                break;

            case BattlePhase.PlayerRollsDice:
                RerollDice(true);
                FreezeUIButtons(false);
                yield return new WaitForSeconds(timeToWait);
                break;
            case BattlePhase.PlayerAction:

                break;
            case BattlePhase.BotAction:
                FreezeUIButtons(true);
                yield return new WaitForSeconds(timeToWait);
                BotTakeAction();
                break;
        }
    }

    public void FreezeUIButtons(bool enabled)
    {
        button_reroll.interactable = !enabled;
        button_end.interactable = !enabled;
    }
    private void BotRollDiceAndSelectTarget()
    {
        RerollDice(false);
        EnemyManager enemyManagerLocal  = enemyManager.GetComponent<EnemyManager>();
        enemyManagerLocal.FindTarget();
        currentPhase = BattlePhase.PlayerRollsDice;
        NextPhase();
    }

    public void PlayerRollDice()
    {
        if (currentPhase == BattlePhase.PlayerRollsDice)
        {
            DrawRerolls();
            RerollDice(true);
            currentRolls--;
            DrawRerolls();
            if (currentRolls <= 0)
            {
                EndButonAction();
            }
        }
    }

    public void EndButonAction()
    {
        if (currentPhase == BattlePhase.PlayerRollsDice)
        {
            button_reroll.interactable = false;
            text_end_reroll.enabled = false;
            text_end_turn.enabled = true;
            button_end.interactable = true;
            currentPhase = BattlePhase.PlayerAction;
            NextPhase();
        }
        else if (currentPhase == BattlePhase.PlayerAction)
        {
            button_reroll.interactable = true;
            text_end_reroll.enabled = true;
            text_end_turn.enabled = false;
            currentPhase = BattlePhase.BotAction;
            NextPhase();
        }
    }


    public void PlayerMakeAction(Unit unit)
    {
        if (currentPhase == BattlePhase.PlayerAction)
        {
            if (current_clicked_unit_1 == null)
            {
                current_clicked_unit_1 = unit.gameObject;
                unit.Text_is_picked.enabled = true;
            }
            else if (current_clicked_unit_1 == unit.gameObject)
            {
                unit.Text_is_picked.enabled = false;
                current_clicked_unit_1 = null;
            }
            else if (current_clicked_unit_2 == null)
            {
                current_clicked_unit_2 = unit.gameObject;
                unit.Text_is_picked.enabled = true; 
            }
            else if (current_clicked_unit_2 == unit.gameObject)
            {
                unit.Text_is_picked.enabled = false;
                current_clicked_unit_2 = null;
            }
        }
    }

    public void EnemyClicked(Unit unit)
    {
        if (currentPhase == BattlePhase.PlayerAction)
        {
            current_clicked_unit_2 = unit.gameObject;
            UnitStats stats = current_clicked_unit_1 != null ? current_clicked_unit_1.GetComponent<Unit>().UnitStats : null;
            current_clicked_unit_1.GetComponent<Unit>().PerformDiceAction(stats.Type.Dice.Current_dice_side, current_clicked_unit_2.GetComponent<Unit>());
            UpdateAllEnemiesStats();
            UpdateAllUnitsStats();
        }
    }

    public void OnUnitDeath(UnitStats unit)
    {
        if (unit.IsPlayerUnit)
        {
            GameManager.Instance.player.units.Remove(unit); // Удаляем юнита
        }
        else
        {
            EnemyManager enemyManagerTemp = enemyManager.GetComponent<EnemyManager>();
            enemyManagerTemp.Enemies.Remove(unit); // Удаляем юнита
        }

        UpdateAllUnitsStats();
        UpdateAllEnemiesStats();

        // Проверка конца игры
        if (!IsAnyTeamAlive())
        {
            currentPhase = BattlePhase.BotAction;
            EndBattle(); // Игрок проиграл
        }
    }

    public void RerollsCount()  
    {
        Player player = GameManager.Instance.player;

        for (int i = 0; i < player.units.Count; i++)
        {
            sumOfReroll += player.units[i].Moral;
        }
        sumOfReroll /= player.units.Count;
    }
    private void DrawRerolls()
    {
        Transform rerollPanel = battleUI.transform.Find("Reroll");
        if (rerollPanel != null)
        {
            Transform textComponents = rerollPanel.transform.Find("Rerolls");
            TextMeshProUGUI rerollText = textComponents.GetComponent<TextMeshProUGUI>();
            if (rerollText != null)
            {
                rerollText.text = currentRolls + "/" + sumOfReroll;
            }
        }
    }


    private void BotTakeAction()
    {
        Debug.Log("Бот сыграл в ответ");
        currentPhase = BattlePhase.BotRollsDice;
        NextPhase();
    }

    private bool IsAnyTeamAlive()
    {
        return  GameManager.Instance.player.units.Count > 0 && enemyManager.GetComponent<EnemyManager>().Enemies.Count > 0;
    }

    private void EndBattle()
    {
        currentPhase = BattlePhase.BotRollsDice;
        gameObject.SetActive(false);
    }


    private IEnumerator SetInteractableCoroutine(bool isFrozen)
    {
        // Получаем или создаем CanvasGroup на родительском объекте
        Transform parentTransform = battleUI.transform;


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
