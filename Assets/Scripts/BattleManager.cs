using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private float timerDuration = 5f; // Длительность таймера
    [SerializeField] private bool isTimerRunning = false; // Состояние таймера
    [SerializeField] public GameObject battleUI; // UI для битвы
    [SerializeField] public Camera mainCamera; // Основная камера
    [SerializeField] public GameObject mapManager; // Менеджер карты
    [SerializeField] public GameObject enemyManager; // Менеджер врагов
    [SerializeField] public GameObject unitPrefab; // Префаб юнита
    [SerializeField] private float diceRollDelay = 0.05f; // Задержка для анимации кубика
    private bool[] diceFrozen;
    private int counter; 

    private void OnEnable()
    {
        StartTimer();
        EnableBattleSystem(true);
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            timerDuration -= Time.deltaTime;
            if (timerDuration <= 0)
            {
                isTimerRunning = false;
                OnTimerFinished();
            }
        }
    }

    private void StartTimer()
    {
        timerDuration = 5f;
        isTimerRunning = true;
    }

    private void OnTimerFinished()
    {
      //  gameObject.SetActive(false); // Деактивируем объект после завершения таймера
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
            DrawUnits();
            DrawEnemies();
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
                    diceImage.sprite = entity.DiceSprite[Random.Range(0, entity.DiceSprite.Length)];
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

    public void DrawEnemies()
    {
        EnemyManager enemyManagerTemp = enemyManager.GetComponent<EnemyManager>();
        enemyManagerTemp.CreateEnemies();
        Transform enemiesPanel = battleUI.transform.Find("Enemies");
        SpawnEntities(enemyManagerTemp.EnemyPrefab, enemiesPanel, enemyManagerTemp.Enemies, false);
    }

    public void RerollDice()
    {
        StartCoroutine(RollDiceCoroutine());
    }

    private IEnumerator RollDiceCoroutine()
    {
        GameManager gameManager = GameManager.Instance;
        Player player = gameManager.player;
        Transform unitsPanel = battleUI.transform.Find("Units");

        if (unitsPanel != null)
        {
            List<Coroutine> diceCoroutines = new List<Coroutine>();

            for (int i = 0; i < player.units.Length; i++)
            {
                Transform unitTransform = unitsPanel.GetChild(i);
                Transform diceSpriteTransform = unitTransform.Find("Dice");
                Image diceImage = diceSpriteTransform?.GetComponent<Image>();

                if (diceImage != null && diceFrozen[i] == false)
                {
                    diceCoroutines.Add(StartCoroutine(RollDiceForUnit(i, diceImage, player.units[i])));
                }
            }

            // Ждем завершения всех корутин
            foreach (var coroutine in diceCoroutines)
            {
                yield return coroutine;
            }
        }
    }

    private IEnumerator RollDiceForUnit(int unitIndex, Image diceImage, Unit unit)
    {
        int randomDiceSide = 0;

        for (int j = 0; j <= 20; j++)
        {
            randomDiceSide = Random.Range(0, unit.DiceSprite.Length);
            diceImage.sprite = unit.DiceSprite[randomDiceSide];
            yield return new WaitForSeconds(diceRollDelay);
        }

        int finalSide = randomDiceSide + 1;
        Debug.Log($"Final dice side for unit {unitIndex}: {finalSide}");
    }

    private void Click(int i)
    {
        diceFrozen[i] = !diceFrozen[i];
    }
}
