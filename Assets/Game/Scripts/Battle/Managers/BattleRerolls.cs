using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleRerolls : MonoBehaviour
{
    public static BattleRerolls Instance { get; private set; }
    public int AvailableRerolls { get => _availableRerolls; set => _availableRerolls = value; }
    public Button EndRerolls { get => _endRerolls; set => _endRerolls = value; }
    public Button RerollButton { get => _rerollButton; set => _rerollButton = value; }

    [SerializeField] private Button _rerollButton;
    [SerializeField] private Button _endRerolls;

    private int _availableRerolls;
    public event System.Action OnAllRerollsComplete;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        RerollButton.onClick.AddListener(OnRerollButtonPressed);
        EndRerolls.onClick.AddListener(OnEndlButtonPressed);
    }

    public void InitRerolls()
    {
        _endRerolls.enabled = true;
        var unitStats = new List<NewUnitStats>();
        foreach (var unit in BattleController.Instance.UnitsObj)
            unitStats.Add(unit.UnitData);

        AvailableRerolls = RerollCalculator.CalculateRerolls(unitStats);
        BattleUI.Instance.ChangeMaxRerollText(AvailableRerolls);
    }

    private void OnRerollButtonPressed()
    {
        if (AvailableRerolls <= 0)
            return;
        BattleDiceManager.Instance.ExecuteRerolls();
        AvailableRerolls--;
        BattleUI.Instance.ChangeMaxRerollText(AvailableRerolls);
        //DeselectPlayerUnits();
    }

    public void DeselectPlayerUnits()
    {
        foreach (var unit in BattleController.Instance.UnitsObj)
            unit.SetSelectionState(false);
    }

    private void OnEndlButtonPressed()
    {
        OnAllRerollsComplete?.Invoke();
        BattleActionManager.Instance.EndAction.gameObject.SetActive(true); 
        AvailableRerolls = 0;
        BattleUI.Instance.ChangeMaxRerollText(AvailableRerolls);
        EndRerolls.gameObject.SetActive(false);
    }
}
