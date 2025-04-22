using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIUnit : MonoBehaviour
{

    [SerializeField] private Image _image;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TooltipTrigger _trigger;

    public Button Button { get => _button; set => _button = value; }

    public void Initialize(NewUnitStats _unit, Action onClick)
    {
        _button.interactable = true;
        _image.sprite = _unit._dice._diceConfig._unitSprite;
        Button.onClick.AddListener(() => onClick?.Invoke());
        _trigger.SetUnitCurrentTooltip(_unit);
        
    }

    public void InitializeBuffs(BuffConfig _buff)
    {
        _image.sprite = _buff.buffSprite;
        _text.text = _buff.buffName;
        _trigger.SetBuffTooltip(_buff);
    }


    public void InitializeUpgrade(NewUnitStats _unitNext,NewUnitStats _unitCurrent, bool state)
    {
        _button.interactable = state;
        _image.sprite = _unitNext._dice._diceConfig._unitSprite;
        Button.onClick.AddListener(() => UpgradeUnit(_unitCurrent, _unitNext));
        _trigger.SetUnitUpgradeTooltip(_unitNext);
    }

    public void CreateUnit()
    {
        if (ResourcesMNG.Instance.TryConsumeResource(ResourcesType.SignalTransmitter, 1))
        {
            TeamMNG.Instance.GenerateAndAddUnit(1);
        }
        else
        {
        }
    }

    public void UpgradeUnit(NewUnitStats _unitCurrent, NewUnitStats _unitNext)
    {
        if (_unitCurrent._current_exp == _unitCurrent._level * 10)
        {
            if (ResourcesMNG.Instance.TryConsumeResource(ResourcesType.SignalTransmitter, _unitCurrent._level + 1))
            {
                TeamMNG.Instance.UpgradePlayerUnit(_unitCurrent._ID, _unitNext._ID);
            }
            else
            {
                Debug.Log("Not enough signal transmitters!");
            }
        }else
        {
            Debug.Log("Not enough exp!");
        }
    }

}
