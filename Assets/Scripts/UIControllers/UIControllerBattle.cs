using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIControllerBattle : MonoBehaviour
{

    [SerializeField] private Transform unitsPanel;
    [SerializeField] private Transform enemiesPanel;
    [SerializeField] private TextMeshProUGUI rerollText;

    BattleManager battleManager = null;
    private int sumOfReroll = 0, currentRolls = 0;

    public Transform UnitsPanel { get => unitsPanel; set => unitsPanel = value; }
    public Transform EnemiesPanel { get => enemiesPanel; set => enemiesPanel = value; }
    public TextMeshProUGUI RerollText { get => rerollText; set => rerollText = value; }
    public int SumOfReroll { get => sumOfReroll; set => sumOfReroll = value; }
    public int CurrentRolls { get => currentRolls; set => currentRolls = value; }


    private void Awake()
    {
        battleManager = BattleManager.Instance.GetComponent<BattleManager>();
    }
    private void SpawnEntities(GameObject entityPrefab, Transform panel, List<UnitStats> entities, bool isPlayerUnit)
    {
        if (panel == null || entityPrefab == null || entities == null) return;

        foreach (Transform child in panel)
        {
            Destroy(child.gameObject); 
        }

        for (int i = 0; i < entities.Count; i++)  
        {
            var entity = entities[i];
            var entityObject = Instantiate(entityPrefab, panel);
            Unit unit = entityObject.GetComponent<Unit>();
            unit.Init(entity);
            unit.UpdateUI(entity,i,unit);

            if (isPlayerUnit)
            {
                battleManager.UnitList.Add(entityObject);
            }
            else
            {
                battleManager.EnemyList.Add(entityObject);
            }
            entityObject.transform.localScale = Vector3.one;
        }
    }

    public void DrawUnits()
    {
        GameManager gameManager = GameManager.Instance;
        SpawnEntities(battleManager.TeamManager.GetComponent<TeamManager>().UnitPrefab, UnitsPanel, gameManager.Player.units, true);
    }

    public void DrawEnemies()
    {
        SpawnEntities(battleManager.EnemyManager.GetComponent<EnemyManager>().EnemyPrefab, EnemiesPanel, battleManager.EnemyManager.GetComponent<EnemyManager>().Enemies, false);
    }
    public void UpdateAllUnitsStats()
    {
        UpdateEntitiesStats(battleManager.UnitList, UnitsPanel);
    }

    public void UpdateAllEnemiesStats()
    {
        UpdateEntitiesStats(battleManager.EnemyList, EnemiesPanel);
    }
    private void UpdateEntitiesStats(List<GameObject> entities, Transform panel)
    {
        // Проверяем, что переданы данные
        if (entities == null || panel == null) return;

        for (int i = 0; i < entities.Count; i++)
        {
            var entity = entities[i];

            entity.GetComponent<Unit>().UiController.UpdateText(entity.GetComponent<Unit>().UnitStats);
        }
    }
    public void DrawRerolls()
    {
        RerollText.text = CurrentRolls + "/" + SumOfReroll;
    }
    public void RerollsCount()
    {
        float tempRoll = 0;
        Player player = GameManager.Instance.Player;

        for (int i = 0; i < player.units.Count; i++)
        {
            tempRoll += player.units[i].Moral;
        }
        tempRoll /= player.units.Count;
        SumOfReroll = (int)Mathf.Ceil(tempRoll);
    }



}
