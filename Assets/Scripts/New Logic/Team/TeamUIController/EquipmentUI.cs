using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    public static EquipmentUI Instance { get; private set; }

    [SerializeField] private List<GameObject> _diceSides;

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

    private void Start()
    {
        UnitsPanelUI.OnUnitSelected += SetupInfo;
    }

    public void SetupInfo(NewUnitStats _clickedUnit)
    {
        if (_diceSides.Count > 0)
        {
            for (int i = 0; i < _diceSides.Count; i++)
            {
                _diceSides[i].GetComponentInChildren<TextMeshProUGUI>().enabled = false;
                _diceSides[i].GetComponent<Image>().sprite = _clickedUnit._dice._diceConfig.sides[i].sprite;

                // Получаем значение с учетом баффов
                int finalPower = CalculateSidePower(_clickedUnit, i);

                if (finalPower != -1)
                {
                    _diceSides[i].GetComponentInChildren<TextMeshProUGUI>().enabled = true;
                    _diceSides[i].GetComponentInChildren<TextMeshProUGUI>().text = finalPower.ToString();
                }
            }
        }
    }

    public int CalculateSidePower(NewUnitStats clickedUnit, int sideIndex)
    {
        if (sideIndex < 0 || sideIndex >= clickedUnit._dice._diceConfig.sides.Count)
        {
            Debug.LogError("Side index out of range");
            return 0;
        }
        int basePower = clickedUnit._dice._diceConfig.sides[sideIndex].power;
        if (basePower == -1)
        {
            return -1;
        }
        ActionType sideType = clickedUnit._dice._diceConfig.sides[sideIndex].actionType;
        int buffBonus = 0;
        int itemBonus = 0;

        foreach (BuffConfig buff in clickedUnit._buffs)
        {
            if (buff.buffType == sideType)
            {
                buffBonus += buff.buffPower;
            }
        }
        foreach (var item in clickedUnit._dice._items) 
        {
            if (item.actionType == sideType) 
            {
                if (item.sideAffect == ItemSideAffect.Even)
                {
                    if ((sideIndex + 1) % 2 == 0)
                    {
                        itemBonus += item.power;
                    }
                }
            }
        }
        
        return basePower + buffBonus + itemBonus ;
    }


}
