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
            // Устанавливаем текстовые значения
            _health.text = _clickedUnit._current_health.ToString() + " / " + _clickedUnit._health.ToString();
            _experience.text = _clickedUnit._current_exp.ToString() + " / " + (_clickedUnit._level * 10).ToString();
            _moral.text = _clickedUnit._currentMoral.ToString();
            UpdateHealthVisuals(_clickedUnit);
        }
    }

    private void UpdateHealthVisuals(NewUnitStats unitData)
    {
        if (_health == null) return;

        float healthPercentage = (float)unitData._current_health / unitData._health;

        // Меняем цвет текста здоровья в зависимости от % здоровья
        if (healthPercentage < 0.4f)
        {
            _health.color = Color.red;
        }
        else if (healthPercentage < 0.7f)
        {
            _health.color = Color.yellow;
        }
        else
        {
            _health.color = Color.green;
        }
    }
}
