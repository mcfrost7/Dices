using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIUnit : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _text;

    public void Initialize(NewDiceConfig _newDiceConfig, UnityAction _callback)
    {
        _image.sprite = _newDiceConfig._unitSprite;
        _button.onClick.AddListener(_callback);
    }

    public void InitializeBuffs(BuffConfig _buff)
    {
        _image.sprite = _buff.buffSprite;
        _text.text = _buff.buffName;
    }
}
