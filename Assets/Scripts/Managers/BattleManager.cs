using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.EventSystems.EventTrigger;
using Unity.VisualScripting;
//using UnityEngine.UIElements;

public class BattleManager : MonoBehaviour
{
    [SerializeField] public GameObject battleUI; // UI для битвы
    [SerializeField] public Camera mainCamera; // Основная камера
    [SerializeField] public GameObject mapManager; // Менеджер карты
    [SerializeField] public GameObject enemyManager; // Менеджер врагов
    [SerializeField] public GameObject unitPrefab; // Префаб юнита
    [SerializeField] private float diceRollDelay = 0.05f; // Задержка для анимации кубика
    [SerializeField] private float timeToWait = 3f;
    private bool[] diceFrozen;
    private int clickedUnits = 0;
    private bool endTurn = false, endRerolls = false;
    private int sumOfReroll = 0, currentRolls = 0;
    private BattlePhase currentPhase = BattlePhase.BotRollsDice;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EnableBattleSystem(true);
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
            DrawEnemies(enemyManagerTemp);
            NextPhase();
        }
    }

    private void SpawnEntities(GameObject entityPrefab, Transform panel, Unit[] entities, bool isPlayerUnit)
    {
        if (panel == null || entityPrefab == null || entities == null) return;

        foreach (Transform child in panel)
        {
            Destroy(child.gameObject);  // Очистка перед спавном новых юнитов
        }

        for (int i = 0; i < entities.Length; i++)  // Добавление индекса в цикл
        {
            var entity = entities[i];
            GameObject entityObject = Instantiate(entityPrefab, panel);

            // Установка изображения
            Transform imgTransform = entityObject.transform.Find("img_marine") ?? entityObject.transform.Find("img_enemy");
            if (imgTransform != null)
            {
                Image imageComponent = imgTransform.GetComponent<Image>();
                if (imageComponent != null)
                {
                    imageComponent.sprite = entity.Sprite;
                }
            }

            // Установка текста здоровья
            Transform hpTransform = entityObject.transform.Find("HP/text_hp");
            if (hpTransform != null)
            {
                TextMeshProUGUI hpText = hpTransform.GetComponent<TextMeshProUGUI>();
                if (hpText != null)
                {
                    hpText.text = entity.CurrentHealth + "/" + entity.Health;
                }
            }

            // Установка текста морали
            Transform moralTransform = entityObject.transform.Find("Moral/text_moral");
            if (moralTransform != null)
            {
                TextMeshProUGUI moralText = moralTransform.GetComponent<TextMeshProUGUI>();
                if (moralText != null)
                {
                    moralText.text = entity.Moral.ToString();
                }
            }

            Transform diceTransform = entityObject.transform.Find("Dice");
            if (diceTransform != null)
            {
                Image diceImage = diceTransform.GetComponent<Image>();
                if (diceImage != null)
                {
                    diceImage.sprite = entity.Type.dice.GetSpriteForAction(DiceConfig.ActionType.Attack);
                }

                if (isPlayerUnit == true)
                {
                    Button diceButton = diceTransform.GetComponent<Button>();
                    if (diceButton != null)
                    {
                        int index = i;
                        diceButton.onClick.AddListener(() => Click(index));  
                    }
                }
            }
            Transform actionTransform = entityObject.transform.Find("ActionTrigger");
            if (actionTransform != null) 
            {
                Button button = actionTransform.GetComponent<Button>();
                if (button != null)
                {
                    int index = i;
                    button.onClick.AddListener(() => PlayerMakeAction(index));
                }
            }
            entityObject.transform.localScale = Vector3.one;
        }
    }

    public void DrawUnits()
    {
        GameManager gameManager = GameManager.Instance;
        Transform unitsPanel = battleUI.transform.Find("Units");
        diceFrozen = new bool[gameManager.player.units.Length];
        for (int i = 0; i < gameManager.player.units.Length; i++)
        {
            diceFrozen[i] = false;
        }
        SpawnEntities(unitPrefab, unitsPanel, gameManager.player.units,true);
    }

    public void DrawEnemies(EnemyManager enemyManagerTemp)
    {
        Transform enemiesPanel = battleUI.transform.Find("Enemies");
        SpawnEntities(enemyManagerTemp.EnemyPrefab, enemiesPanel, enemyManagerTemp.Enemies, false);
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
        Unit[] units = isPlayer ? gameManager.player.units : enemyManager.GetComponent<EnemyManager>().Enemies;

        if (panel != null)
        {
            List<Coroutine> diceCoroutines = new List<Coroutine>();

            for (int i = 0; i < units.Length; i++)
            {
                Transform unitTransform = panel.GetChild(i);
                Transform diceSpriteTransform = unitTransform.Find("Dice");
                Image diceImage = diceSpriteTransform?.GetComponent<Image>();

                if (diceImage != null && (isPlayer ? !diceFrozen[i] : true))
                {
                    diceCoroutines.Add(StartCoroutine(RollDiceForUnit(i, diceImage, units[i])));
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
        DiceConfig diceConfig = unit.Type.dice; // Получаем конфиг дайса юнита

        if (diceConfig == null || diceConfig.actionSprites.Length == 0)
        {
            Debug.LogWarning("DiceConfig or actionSprites is missing for unit.");
            yield break; // Прекращаем выполнение, если конфиг или спрайты отсутствуют
        }

        for (int j = 0; j <= 20; j++)
        {
            // Генерируем случайный индекс для выбора стороны дайса
            randomDiceSide = Random.Range(0, diceConfig.actionSprites.Length);
            diceImage.sprite = diceConfig.actionSprites[randomDiceSide];
            yield return new WaitForSeconds(diceRollDelay);
        }

        int finalSide = randomDiceSide + 1;
        Debug.Log($"Final dice side for unit {unitIndex}: {finalSide}");
    }


    private void Click(int i)
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
                endTurn = false;
                endRerolls = false;
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
        Transform buttonRerollTransform = battleUI.transform.Find("Reroll");
        Button buttonReroll = buttonRerollTransform.GetComponent<Button>();
        Transform buttonEndTrasnsform = battleUI.transform.Find("End");
        Button buttonEnd = buttonRerollTransform.GetComponent<Button>();
        Transform buttonActionTriggerTrasnsform = battleUI.transform.Find("ActionTrigger");
        Button buttonActionTrigger = buttonRerollTransform.GetComponent<Button>();
        buttonReroll.interactable = !enabled;
        buttonEnd.interactable = !enabled;
        buttonActionTrigger.interactable= !enabled;
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
        if (currentPhase == BattlePhase.PlayerRollsDice )
        {
            DrawRerolls();
            RerollDice(true);
            currentRolls--;
            DrawRerolls();
            if (currentRolls <= 0)
            {
                Transform button = battleUI.transform.Find("Reroll");
                if (button != null)
                {
                    Button buttonSettings = button.GetComponent<Button>();
                    buttonSettings.interactable = false;
                    currentPhase = BattlePhase.PlayerAction;
                    NextPhase();
                }
            }
        }
    }


    public void EndButonAction()
    {
        Transform endButton = battleUI.transform.Find("End");
        TextMeshProUGUI rerollText = endButton.transform.Find("RerollText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI endTurnText = endButton.transform.Find("EndTurnText").GetComponent<TextMeshProUGUI>();
        if (rerollText.enabled == true)
        {
            rerollText.enabled = false;
            endTurnText.enabled = true;
            endRerolls = true;
            currentPhase = BattlePhase.PlayerAction;
            NextPhase();
        }
        else
        {
            rerollText.enabled = true;
            endTurnText.enabled = false;
            endTurn = true;
            Debug.Log("Отыграл бой");
            currentPhase = BattlePhase.BotAction;
            NextPhase();
        }
    }

    private void PlayerMakeAction(int index)
    {
        Debug.Log("Нажата");

        if (currentPhase == BattlePhase.PlayerAction)
        {
            Transform textTrigger = battleUI.transform.Find("");

            if (textTrigger != null)
            {
                TextMeshProUGUI text = textTrigger.GetComponent<TextMeshProUGUI>();

                if (text != null)
                {
                    if (!text.enabled)
                    {
                        text.enabled = true;
                        clickedUnits++;
                    }
                    else
                    {
                        text.enabled = false;
                        clickedUnits--;
                    }
                }
                else
                {
                    Debug.LogError("TextMeshProUGUI компонент не найден на объекте IsPicked");
                }
            }
            else
            {
                Debug.LogError("Объект с именем IsPicked не найден");
            }
        }

        if (clickedUnits == 5)
        {
            currentPhase = BattlePhase.BotAction;
            NextPhase();
        }
    }


    public void RerollsCount()
    {
        Player player = GameManager.Instance.player;

        for (int i = 0; i < player.units.Length; i++)
        {
            sumOfReroll += player.units[i].Moral;
        }
        sumOfReroll /= player.units.Length;
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
        return  GameManager.Instance.player.units.Length > 0 && enemyManager.GetComponent<EnemyManager>().Enemies.Length > 0;
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
