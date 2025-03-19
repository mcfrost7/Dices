using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _moralText;
    [SerializeField] private Image _unitImage;
    [SerializeField] private Image _diceImage;
    [SerializeField] private Button _actionTrigger;
    [SerializeField] private GameObject _linePoint;

    private NewUnitStats unitData;

    public NewUnitStats UnitData { get => unitData; set => unitData = value; }
    public GameObject LinePoint { get => _linePoint; set => _linePoint = value; }

    public void SetupUnit(NewUnitStats newUnitStats)
    {
        _healthText.text = newUnitStats._current_health.ToString() + "/" + newUnitStats._health.ToString();
        _moralText.text = newUnitStats._moral.ToString();
        _unitImage.sprite = newUnitStats._dice._diceConfig._unitSprite;
        _diceImage.sprite = newUnitStats._dice.GetCurrentSide().sprite;
        _actionTrigger.enabled = false;
        UnitData = newUnitStats;
    }
    public void SetupEnemy(NewUnitStats newUnitStats)
    {
        _healthText.text = newUnitStats._current_health.ToString() + "/" + newUnitStats._health.ToString();
        _unitImage.sprite = newUnitStats._dice._diceConfig._unitSprite;
        _diceImage.sprite = newUnitStats._dice._diceConfig.sides[Random.Range(0, 6)].sprite;
        _actionTrigger.enabled = false;
        UnitData = newUnitStats;
    }
}
