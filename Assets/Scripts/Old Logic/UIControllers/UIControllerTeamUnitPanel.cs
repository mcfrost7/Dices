using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerTeamUnitPanel : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject unitPanel;
    [SerializeField] private GameObject unitTeamPrefab;
    [SerializeField] private GameObject addButton;
    [SerializeField] private TeamManager teamManager;
    [SerializeField] private UIControllerTeamUnitStats uIControllerTeamUnitStats;

    private void OnEnable()
    {
        if (teamManager == null)
        {
            Debug.LogError("TeamManager не назначен!");
            return;
        }
        if (teamManager.GetPlayerUnits().Count <= 4)
        {
            addButton.gameObject.SetActive(true);
        }

        canvas.gameObject.SetActive(true);
        DrawAllUnits();

    }

    private void OnDisable()
    {
        if (canvas != null)
            canvas.gameObject.SetActive(false);
    }

    private void DrawAllUnits()
    {
        // Удаляем старые объекты, кроме кнопки "Добавить"
        foreach (Transform child in unitPanel.transform)
        {
            if (child.gameObject.name != "AddButton")
            {
                Destroy(child.gameObject);
            }
        }

        // Отображаем юниты из команды
        List<UnitStats> units = teamManager.GetPlayerUnits();
        foreach (var unit in units)
        {
            DrawUnit(unit);
            OnUnitButtonClicked(unit);
        }

            
    }

    private void DrawUnit(UnitStats unitStats)
    {
        // Проверяем, что ссылки на unitPanel и unitTeamPrefab установлены
        if (unitPanel == null || unitTeamPrefab == null)
        {
            Debug.LogError("unitPanel или unitTeamPrefab не назначены!");
            return;
        }

        // Создаем объект и добавляем его в панель
        GameObject newUnit = Instantiate(unitTeamPrefab, unitPanel.transform);

        // Настраиваем объект: загружаем спрайт в Image
        Image unitImage = newUnit.GetComponent<Image>();
        if (unitImage != null && unitStats.Sprite != null)
        {
            unitImage.sprite = unitStats.Sprite;
        }
        else
        {
            Debug.LogWarning("Компонент Image не найден у unitTeamPrefab или Sprite_type отсутствует в UnitStats.");
        }

        // Получаем компонент Button и добавляем слушатель
        Button unitButton = newUnit.GetComponent<Button>();
        if (unitButton != null)
        {
            unitButton.onClick.AddListener(() => OnUnitButtonClicked(unitStats));
        }
        else
        {
            Debug.LogWarning("Компонент Button не найден у unitTeamPrefab.");
        }
        Debug.Log($"Юнит с именем {unitStats.Type.TypeName} добавлен на панель.");
    }

    private void OnUnitButtonClicked(UnitStats unitStats)
    {
        uIControllerTeamUnitStats.GetComponent<UIControllerTeamUnitStats>().DrawAllPanels(unitStats);
        uIControllerTeamUnitStats.GetComponent<UIControllerTeamInventory>().DrawAllPanels(unitStats);

    }

    public void OnAddUnitButtonClicked()
    {
        if (teamManager.GetPlayerUnits().Count < 5) // Лимит на 5 юнита
        {
            teamManager.CreateUnit(1);
            DrawAllUnits();
        }
        if (teamManager.GetPlayerUnits().Count == 5)
        {
            addButton.gameObject.SetActive(false);
        }
    }
}
