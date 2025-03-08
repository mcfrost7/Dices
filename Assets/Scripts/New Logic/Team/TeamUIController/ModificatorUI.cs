using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModificatorUI : MonoBehaviour
{
    [SerializeField] private GameObject _modificatorPrefab;
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _defaultText;


    private void Start()
    {
        UnitsPanelUI.OnUnitSelected += SetUpInfo;
    }

    public void SetUpInfo(NewUnitStats _clickedUnit)
    {
        ClearPanel();
        if (_clickedUnit._buffs.Count > 0)
        {
            if (_modificatorPrefab != null && _panel != null)
            {
                for (int i = 0; i < _clickedUnit._buffs.Count; i++)
                {
                    GameObject _modificatorOnPanel = Instantiate(_modificatorPrefab, _panel.transform);
                    Image image = _modificatorOnPanel.GetComponentInChildren<Image>();
                    if (image != null)
                    {
                        image.sprite = _clickedUnit._buffs[i].buffSprite;
                    }
                    TextMeshProUGUI name = _modificatorOnPanel.GetComponentInChildren<TextMeshProUGUI>();
                    if (name != null)
                    {
                        name.text = _clickedUnit._buffs[i].buffName;
                    }
                }
            }
        }
        else
        {
            _defaultText?.SetActive(true);
        }

    }

    private void ClearPanel()
    {
        if (_panel != null)
        {
            foreach (Transform child in _panel.transform)
            {
                Destroy(child.gameObject);
            }
        }
        _defaultText?.SetActive(false);
    }
}
