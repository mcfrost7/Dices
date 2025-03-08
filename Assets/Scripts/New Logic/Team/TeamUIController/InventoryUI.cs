using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{

    [SerializeField] private List<GameObject> _diceSides;
    [SerializeField] private List<GameObject> _itemSides;

    private void Start()
    {
        UnitsPanelUI.OnUnitSelected += SetUpInfo;
    }

    private void SetUpInfo(NewUnitStats _clickedUnit)
    {
        SetUpItems(_clickedUnit);
        if (_diceSides.Count > 0) 
        {
            for (int i = 0;  i < _diceSides.Count; i++)
            {
                _diceSides[i].GetComponentInChildren<TextMeshProUGUI>().enabled = false;
                _diceSides[i].GetComponent<Image>().sprite = _clickedUnit._dice.diceConfig.sides[i].sprite;
                if (_clickedUnit._dice.diceConfig.sides[i].power != -1)
                {
                    _diceSides[i].GetComponentInChildren<TextMeshProUGUI>().enabled = true;
                    _diceSides[i].GetComponentInChildren<TextMeshProUGUI>().text = _clickedUnit._dice.diceConfig.sides[i].power.ToString(); 
                }
            }
        }
    }

    private void SetUpItems(NewUnitStats _clickedUnit)
    {
        if (_itemSides.Count > 0)
        {
            for (int i = 0; i < _itemSides.Count; i++)
            {
                _itemSides[i].GetComponent<Image>().enabled = false;
                if (_clickedUnit._dice.items.Count > 0)
                {
                    _itemSides[i].SetActive(true);
                    _itemSides[i].GetComponent<Image>().sprite = _clickedUnit._dice.items[i].icon;
                }
            }
        }
    }
}
