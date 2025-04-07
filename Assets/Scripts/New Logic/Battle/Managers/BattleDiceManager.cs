using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleDiceManager : MonoBehaviour
{
    public event System.Action<BattleUnit> UnitSelected;
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

    public static BattleDiceManager Instance { get; private set; }

    public event System.Action OnAllRollsComplete;
    public event System.Action OnAllRerollsComplete;
    public bool AllowMultipleSelections { get; set; } = true;
    public void RollAllDice()
    {
        DisableAllUnitSelections();
        int totalRolls = BattleController.Instance.UnitsObj.Count + BattleController.Instance.EnemiesObj.Count;
        int completedRolls = 0;

        System.Action onSingleRollComplete = () =>
        {
            completedRolls++;
            Debug.Log($"Roll completed: {completedRolls}/{totalRolls}");
            if (completedRolls >= totalRolls && OnAllRollsComplete != null)
            {
                Debug.Log("All rolls complete, invoking callback");
                EnablePlayerUnitSelections(); // Enable unit selections when all rolls are complete
                OnAllRollsComplete.Invoke();
            }
        };

        foreach (BattleUnit unit in BattleController.Instance.UnitsObj)
        {
            if (unit != null)
            {
                Debug.Log($"Starting roll animation for player unit: {unit.name}");
                StartCoroutine(RollDiceAnimation(unit, onSingleRollComplete));
            }
            else
            {
                Debug.LogError("Null player unit found in UnitsObj list");
                onSingleRollComplete();
            }
        }

        foreach (BattleUnit enemy in BattleController.Instance.EnemiesObj)
        {
            if (enemy != null)
            {
                Debug.Log($"Starting roll animation for enemy unit: {enemy.name}");
                StartCoroutine(RollDiceAnimation(enemy, onSingleRollComplete));
            }
            else
            {
                Debug.LogError("Null enemy unit found in EnemiesObj list");
                onSingleRollComplete();
            }
        }

        if (totalRolls == 0 && OnAllRollsComplete != null)
        {
            Debug.LogWarning("No units to roll dice for!");
            OnAllRollsComplete.Invoke();
        }
    }

    public void ExecuteRerolls()
    {

        DisableAllUnitSelections();
        List<BattleUnit> selectedUnits = GetSelectedUnitsForReroll();
        List<BattleUnit> unitsToReroll = new List<BattleUnit>();


        foreach (BattleUnit unit in BattleController.Instance.UnitsObj)
        {
            if (!selectedUnits.Contains(unit))
            {
                unitsToReroll.Add(unit);
                Debug.Log($"Unit {unit.name} marked for reroll");
            }
        }

        int totalRerolls = unitsToReroll.Count;
        int completedRerolls = 0;

        Debug.Log($"Total units to reroll: {totalRerolls}");

        if (totalRerolls == 0)
        {
            Debug.Log("No rerolls needed, completing immediately");
            if (OnAllRerollsComplete != null)
            {

                OnAllRerollsComplete.Invoke();
            }
            return;
        }

        System.Action onSingleRerollComplete = () =>
        {
            completedRerolls++;
            Debug.Log($"Reroll completed: {completedRerolls}/{totalRerolls}");
            if (completedRerolls >= totalRerolls && OnAllRerollsComplete != null)
            {
                Debug.Log("All rerolls complete, invoking callback");
                EnablePlayerUnitSelections();
                // Enable unit selections when all rerolls are complete
                OnAllRerollsComplete.Invoke();
            }
        };

        foreach (BattleUnit unit in unitsToReroll)
        {
            if (unit != null)
            {
                Debug.Log($"Starting reroll animation for unit: {unit.name}");
                StartCoroutine(RollDiceAnimation(unit, onSingleRerollComplete));
            }
            else
            {
                Debug.LogError("Null unit found in unitsToReroll list");
                onSingleRerollComplete();
            }
        }
        
    }
    public void HandleUnitSelectionChanged(BattleUnit unit)
    {
        Debug.Log($"Unit {unit.name} selection changed: {unit.IsSelected}");

        bool allowMultiple = AllowMultipleSelections;

        if (unit.IsSelected)
        {
            if (!allowMultiple)
            {
                foreach (BattleUnit otherUnit in BattleController.Instance.UnitsObj)
                {
                    if (otherUnit != unit && otherUnit.IsSelected)
                    {
                        otherUnit.SetSelectionState(false);
                    }
                }
            }

            UnitSelected?.Invoke(unit);
        }
        else
        {
            UnitSelected?.Invoke(null);
        }
    }


    private List<BattleUnit> GetSelectedUnitsForReroll()
    {
        List<BattleUnit> selectedUnits = new List<BattleUnit>();

        foreach (BattleUnit unit in BattleController.Instance.UnitsObj)
        {
            if (unit != null && unit.IsSelected)
            {
                selectedUnits.Add(unit);
            }
        }

        return selectedUnits;
    }

    private IEnumerator RollDiceAnimation(BattleUnit unit, System.Action onComplete)
    {
        if (unit == null || unit.DiceImage == null || unit.UnitData == null || unit.UnitData._dice == null)
        {
            Debug.LogError("Invalid unit or missing components for dice roll animation");
            if (onComplete != null)
            {
                onComplete();
            }
            yield break;
        }

        Image diceImage = unit.DiceImage;
        Dice dice = unit.UnitData._dice;
        NewDiceConfig diceConfig = dice._diceConfig;
        TextMeshProUGUI power = unit.PowerText;

        if (diceConfig == null || diceConfig.sides == null || diceConfig.sides.Count == 0)
        {
            Debug.LogError("Invalid dice config or missing sides");
            if (onComplete != null)
            {
                onComplete();
            }
            yield break;
        }

        float duration = 3.0f;
        float elapsed = 0f;
        float animationSpeed = 10f;

        List<DiceSide> sides = diceConfig.sides;
        int currentSideIndex = 0;

        while (elapsed < duration)
        {
            diceImage.sprite = sides[currentSideIndex].sprite;
            power.text = unit.CalculateSidePowerWithBuffs(unit.UnitData, sides[currentSideIndex]).ToString();
            currentSideIndex = (currentSideIndex + 1) % sides.Count;
            yield return new WaitForSeconds(1f / animationSpeed);
            elapsed += 1f / animationSpeed;

            if (elapsed > duration * 0.7f)
            {
                animationSpeed *= 0.95f;
            }
        }

        int randomSideIndex = Random.Range(0, sides.Count);
        DiceSide finalSide = sides[randomSideIndex];
        power.text = unit.CalculateSidePowerWithBuffs(unit.UnitData, finalSide).ToString();
        diceImage.sprite = finalSide.sprite;
        dice.SetCurrentSide(finalSide);

        if (onComplete != null)
        {
            onComplete();
        }
    }

    public void DisableAllUnitSelections()
    {
        SetRerollButtonsActive(false);
        foreach (BattleUnit unit in BattleController.Instance.UnitsObj)
        {
            if (unit != null && unit.ActionTrigger != null)
            {
                unit.ActionTrigger.enabled = false;
            }
        }
    }

    public void EnablePlayerUnitSelections()
    {
        SetRerollButtonsActive(true);
        foreach (BattleUnit unit in BattleController.Instance.UnitsObj)
        {
            if (unit != null && unit.ActionTrigger != null)
            {
                unit.ActionTrigger.enabled = true;
            }
        }
    }

    private static void SetRerollButtonsActive(bool _state)
    {
        BattleRerolls.Instance.RerollButton.enabled = _state;
        BattleRerolls.Instance.EndRerolls.enabled = _state;
    }

    public void NotifyUnitSelected(BattleUnit unit)
    {
        UnitSelected?.Invoke(unit);
    }
}
