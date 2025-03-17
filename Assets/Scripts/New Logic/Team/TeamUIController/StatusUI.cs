using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusUI : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI _health;
    [SerializeField]private TextMeshProUGUI _experience;
    [SerializeField]private TextMeshProUGUI _moral;


    private void Start()
    {
        UnitsPanelUI.OnUnitSelected += SetUpInfo;
    }

    public void SetUpInfo(NewUnitStats _clickedUnit)
    {
        if (_health != null && _experience != null && _moral != null)
        {
            _health.text = _clickedUnit._current_health.ToString() + " / " + _clickedUnit._health.ToString();
            _experience.text = _clickedUnit._current_exp.ToString();
            _moral.text = _clickedUnit._moral.ToString();
        }
    }
}
