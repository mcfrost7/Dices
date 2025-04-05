using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleRerolls : MonoBehaviour
{
    public static BattleRerolls Instance { get; private set; }

    [SerializeField] private Button _rerollButton;

    private int _availableRerolls;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        _rerollButton.onClick.AddListener(OnRerollButtonPressed);
    }

    public void InitRerolls()
    {
        var unitStats = new List<NewUnitStats>();
        foreach (var unit in BattleController.Instance.UnitsObj)
            unitStats.Add(unit.UnitData);

        _availableRerolls = RerollCalculator.CalculateRerolls(unitStats);
        BattleUI.Instance.ChangeMaxRerollText(_availableRerolls);
    }

    private void OnRerollButtonPressed()
    {
        if (_availableRerolls <= 0)
            return;

        BattleDiceManager.Instance.ExecuteRerolls();
        _availableRerolls--;
        BattleUI.Instance.ChangeMaxRerollText(_availableRerolls);
    }
}
