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


    public void InitializeUpgrade(NewUnitStats _unitNext,NewUnitStats _unitCurrent)
    {
        _image.sprite = _unitNext._dice.diceConfig._unitSprite;
        _button.onClick.AddListener(() => TeamMNG.Instance.UpgradePlayerUnit(_unitCurrent._ID, _unitNext._ID));
    }

    public void CreateUnit()
    {
        TeamMNG.Instance.GenerateAndAddUnit(1);
    }
}
