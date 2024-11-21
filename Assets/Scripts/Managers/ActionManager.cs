using UnityEngine;

public class ActionManager : MonoBehaviour
{
    private Unit activeUnit; // Юнит, который совершает действие
    private Unit targetUnit; // Юнит, над которым совершается действие

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Сброс выделения по нажатию на К.");
            ResetSelection();
            return;
        }
    }

    public void SelectUnit(Unit unit)
    {
        // Если уже выбран активный юнит, то выбираем цель
        if (activeUnit == null)
        {
            if (unit.Can_act) // Проверяем, может ли юнит выполнять действия
            {
                activeUnit = unit;
                HighlightUnit(unit, true); // Подсвечиваем активного юнита
                Debug.Log($"Активный юнит выбран: {unit.name}");
            }
            else
            {
                Debug.Log($"Юнит {unit.name} не может действовать.");
            }
        }
        else
        {
            // Если активный юнит уже выбран, выбираем цель
            targetUnit = unit;

            // Выполняем действие
            PerformAction(activeUnit, targetUnit);

            // Сбрасываем выбор
            ResetSelection();
        }
    }

    public void PerformAction(Unit active, Unit target)
    {
        if (target == null)
        {
            Debug.Log("Цель не выбрана.");
            return;
        }

        // Пример выполнения действия
        Debug.Log($"{active.name} атакует {target.name}");
        active.PerformDiceAction(active.UnitStats.Current_dice_side,target);

        // Обновление состояния активного юнита
        active.Can_act = false;
        if (active.UnitStats.IsPlayerUnit == true) 
            SetUnitInteractable(active); 
    }

    private void ResetSelection()
    {
        if (activeUnit != null)
        {
            HighlightUnit(activeUnit, false); 
        }

        activeUnit = null;
        targetUnit = null;
        Debug.Log("Выделение сброшено.");
    }

    private void HighlightUnit(Unit unit, bool highlight)
    {
        if (unit != null)
        {
           unit.UiController.UpdatePickStatus(highlight);
        }
    }

    private void SetUnitInteractable(Unit unit)
    {
        if (unit != null)
        {
            CanvasGroup unitCanvasGroup = unit.GetComponent<CanvasGroup>();
            if (unitCanvasGroup == null)
            {
                unitCanvasGroup = unit.gameObject.AddComponent<CanvasGroup>();
            }

            unitCanvasGroup.interactable = unit.Can_be_interacted;
            unitCanvasGroup.blocksRaycasts = unit.Can_be_interacted;
            unitCanvasGroup.alpha = unit.Can_act ? 1f : 0.5f;
        }
    }
    public void EndTurn()
    {
        Debug.Log("Окончание хода. Обновление всех юнитов.");

        // Размораживаем все юниты
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            unit.Can_act = true;
            unit.Can_be_interacted = true;
            SetUnitInteractable(unit); // Обновляем визуальное состояние
        }
    }

}
