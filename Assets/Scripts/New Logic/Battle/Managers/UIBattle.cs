using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleUI : MonoBehaviour
{
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
    public static BattleUI Instance { get; private set; }

    public GameObject arrowPrefab; // Префаб стрелки (UI Image)
    public RectTransform arrowContainer; // Контейнер для стрелок в Canvasов
    private List<GameObject> arrows = new List<GameObject>();

    public void ShowIntention(Dictionary<NewUnitStats, NewUnitStats> enemyIntentions)
    {
        // Clear previous arrows
        ClearArrows();

        // Go through each enemy object directly
        foreach (var enemyObj in BattleController.Instance.EnemiesObj)
        {

            // Get the enemy's stats
            var enemyUnit = enemyObj.UnitData;

            // Skip if not found in intentions dictionary
            if (!enemyIntentions.TryGetValue(enemyUnit, out var targetUnit))
                continue;

            // Find target object
            var targetObj = BattleController.Instance.UnitsObj
                .FirstOrDefault(unit => unit.UnitData._name == targetUnit._name);

            if (targetObj == null)
                continue;

            // Get connection points
            var enemyLinePoint = enemyObj.LinePoint;
            var targetLinePoint = targetObj.LinePoint;
            Vector3 enemyWorldPos = enemyLinePoint.transform.position;

            // Add offset based on the enemy's position in the layout group
            RectTransform enemyRect = enemyObj.GetComponent<RectTransform>();
            enemyWorldPos = enemyObj.transform.position + new Vector3(enemyRect.rect.width / 2, 0, 0);

            if (enemyLinePoint == null || targetLinePoint == null)
                continue;

            Vector3 targetWorldPos = targetLinePoint.transform.position;

            Vector2 enemyCanvasPos = arrowContainer.InverseTransformPoint(enemyWorldPos);
            Vector2 targetCanvasPos = arrowContainer.InverseTransformPoint(targetWorldPos);

            CreateArrow(enemyCanvasPos, targetCanvasPos);
        }
    }

    private void CreateArrow(Vector2 enemyCanvasPos, Vector2 targetCanvasPos)
    {
        // Instantiate arrow from prefab
        GameObject arrowObj = Instantiate(arrowPrefab, arrowContainer);
        // Add to list for later cleanup
        arrows.Add(arrowObj);

        // Get the RectTransform of the arrow
        RectTransform rectTransform = arrowObj.GetComponent<RectTransform>();

        // Calculate direction and distance
        Vector2 direction = targetCanvasPos - enemyCanvasPos;  // Direction from enemy to target
        float distance = direction.magnitude;

        // Set pivot to the enemy's position
        rectTransform.pivot = new Vector2(0.5f, 1f);

        // Set the position to the enemy's position
        rectTransform.localPosition = new Vector3(enemyCanvasPos.x, enemyCanvasPos.y, 0f);

        // Set the size of the arrow (width = arrow width, height = distance between enemy and target)
        rectTransform.sizeDelta = new Vector2(10f, distance);

        // Set rotation based on direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectTransform.localRotation = Quaternion.Euler(0, 0, angle); // Correct orientation

        // Debugging logs (optional)
        Debug.Log("Arrow created from enemy at " + enemyCanvasPos + " to target at " + targetCanvasPos);
    }


    private void ClearArrows()
    {
        foreach (GameObject arrow in arrows)
        {
            Destroy(arrow);
        }
        arrows.Clear();
    }
    public void HideIntention() { }
    public bool AreIntentionsConfirmed() { return false; }

    public void ShowDiceRolling() { }
    public void ShowDiceResults() { }

    public void ShowRerollPanel() { }
    public void HideRerollPanel() { }
    public bool AreRerollsConfirmed() { return false; }

    public void ShowActionPanel() { }
    public void HideActionPanel() { }
    public bool AreActionsComplete() { return false; }

    public void ShowVictoryScreen() { }
    public void HideVictoryScreen() { }

    public void ShowDefeatScreen() { }
    public void HideDefeatScreen() { }
}
