using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;
using static Unity.VisualScripting.Member;

public class BattleActionManager : MonoBehaviour
{
    [SerializeField] private Button _endAction;
    [SerializeField] private Button _deselect;
    [SerializeField] private SelectedSide _selectedSide;
    [SerializeField] private TextMeshProUGUI _stateText;

    private BattleUnit _selectedUnit;
    private BattleUnit _targetUnit;
    private bool _isWaitingForTarget = false;

    public event System.Action ActionComplete;
    public static BattleActionManager Instance { get; private set; }
    public Button EndAction { get => _endAction; set => _endAction = value; }
    public bool IsWaitingForTarget { get => _isWaitingForTarget; }

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

        _endAction.onClick.AddListener(EndActionButton);
        _deselect.onClick.AddListener(DeselectUnit);

        if (_selectedSide != null && _stateText != null)
        {
            HideActionFeedback();
            HideStateText();
        }
    }

    public void EnableEnemyUnitSelection()
    {
        foreach (BattleUnit unit in BattleController.Instance.EnemiesObj)
        {
            if (unit != null && unit.ActionTrigger != null)
            {
                unit.ActionTrigger.enabled = true;
            }
        }
    }

    public void SetSelectedUnit(BattleUnit unit)
    {
        // Не позволяем выбирать использованного юнита как исполнителя
        if (unit.IsUsed)
        {
            return;
        }

        _selectedUnit = unit;
        _isWaitingForTarget = true;
        ShowActionFeedback(_selectedUnit);
    }

    public void SetTargetUnit(BattleUnit unit)
    {
        if (!_isWaitingForTarget || _selectedUnit == null)
            return;

        _targetUnit = unit;

        DiceSide currentSide = _selectedUnit.UnitData._dice.GetCurrentSide();
        bool isSelfTargetingAllyAction = (_selectedUnit == _targetUnit &&
                                         currentSide.ActionSide == ActionSide.Ally);

        if (IsValidTarget(_selectedUnit, _targetUnit) || isSelfTargetingAllyAction)
        {
            ExecuteAction(_selectedUnit, _targetUnit);
            _isWaitingForTarget = false;
            HideActionFeedback();
        }
        else
        {
            return;
        }
    }

    public void ExecuteAction(BattleUnit source, BattleUnit target)
    {
        if (source == null || target == null)
            return;
        DiceSide currentSide = source.UnitData._dice.GetCurrentSide();
        int power = source.CalculateSidePower(source.UnitData, currentSide);
        int duration = source.UnitData._dice.GetCurrentSide().duration;
        IBattleAction action = BattleActionFactory.Instance.GetAction(currentSide.actionType);
        action.Execute(source, target, power,duration);
        if (source.IsEnemy == false)
        {
            SFXManager.Instance.PlaySoundSM(currentSide.actionType);
        }
        else
        {
            SFXManager.Instance.PlaySoundOrk(currentSide.actionType);
        }

        source.DisableUnitAfterAction();
        source.RefreshUnitUI();
        target.RefreshUnitUI();
        DeselectUnit();
        CheckForDefeat(target);
        CheckForDefeat(source);
    }

    private bool IsValidTarget(BattleUnit source, BattleUnit target)
    {
        if (source == null || target == null || source.UnitData == null || target.UnitData == null)
            return false;

        DiceSide currentSide = source.UnitData._dice.GetCurrentSide();
        ActionType actionType = currentSide.actionType;

        if (source == target && currentSide.ActionSide == ActionSide.Ally)
            return true;

        IBattleAction action = BattleActionFactory.Instance.GetAction(actionType);
        return action.IsValidTarget(source, target);
    }

    private void CheckForDefeat(BattleUnit target)
    {
        if (target.UnitData._current_health <= 0)
        {
            target.Die();
        }
    }

    public void PlayDeathAnimation(BattleUnit unit)
    {
        StartCoroutine(DeathAnimation(unit));
    }

    private IEnumerator DeathAnimation(BattleUnit unit)
    {
        int direction = unit.IsEnemy ? 1 : -1;
        RectTransform rect = unit.GetComponent<RectTransform>();
        var layoutGroup = rect.parent.GetComponent<HorizontalOrVerticalLayoutGroup>();
        var fitter = rect.parent.GetComponent<ContentSizeFitter>();
        var layoutElem = unit.GetComponent<LayoutElement>();

        if (layoutGroup != null) layoutGroup.enabled = false;
        if (fitter != null) fitter.enabled = false;
        if (layoutElem != null) layoutElem.ignoreLayout = true;

        Vector2 startPos = rect.anchoredPosition;
        rect.DOAnchorPosX(startPos.x + 560 * direction, 1f).SetEase(Ease.InOutExpo);
        yield return new WaitForSeconds(1f);
        if (unit.IsEnemy)
        {
            SFXManager.Instance.PlaySoundOrk(ActionType.Death);
        }
        else
        {
            SFXManager.Instance.PlaySoundSM(ActionType.Death);
        }
        if (layoutElem != null) layoutElem.ignoreLayout = false;
        if (layoutGroup != null) layoutGroup.enabled = true;
        if (fitter != null) fitter.enabled = true;

        Destroy(unit.gameObject);

        BattleUI.Instance.ShowIntentionDelayed(BattleEnemyAI.Instance.EnemyIntentions);
    }


    public void ShowStateText(string message)
    {
        if (_stateText != null)
        {
            _stateText.gameObject.SetActive(true);
            _stateText.text = message;
        }
    }

    public void HideStateText()
    {
        if (_stateText != null)
        {
            _stateText.gameObject.SetActive(false);
        }
    }

    public void ShowActionFeedback(BattleUnit unit)
    {
        if (_selectedSide != null)
        {
            _selectedSide.gameObject.SetActive(true);
            _selectedSide.SetupInfo(unit);
        }
    }

    public void HideActionFeedback()
    {
        if (_selectedSide != null)
        {
            _selectedSide.gameObject.SetActive(false);
        }
    }

    public void EndActionButton()
    {
        ActionComplete?.Invoke();
        _endAction.gameObject.SetActive(false);
        BattleRerolls.Instance.EndRerolls.gameObject.SetActive(true);
        BattleRerolls.Instance.EndRerolls.enabled = false;
        foreach (var unit in BattleController.Instance.UnitsObj)
        {
            unit.EnableUnitForNewTurn();
        }
        foreach (var unit in BattleController.Instance.EnemiesObj)
        {
            unit.EnableUnitForNewTurn();
        }
    }

    public void DeselectUnit()
    {
        if (_selectedUnit != null)
        {
            _selectedUnit.SetSelectionState(false);
            _selectedUnit = null;
        }
        if (_targetUnit != null)
        {
            _targetUnit.SetSelectionState(false);
            _targetUnit = null;
        }
        _isWaitingForTarget = false;
        HideActionFeedback();
    }
}