using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampPanel : MonoBehaviour
{
    public static CampPanel Instance { get; private set; }
    public Button Button { get => _button; set => _button = value; }

    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _text;

    private bool _campVisibility = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void CallTask()
    {
        if (_campVisibility == false)
        {
            _panel.transform.DOLocalMoveX(_panel.transform.localPosition.x - 500, 0.8f).SetEase(Ease.InOutExpo);
        } else
        {
            _panel.transform.DOLocalMoveX(_panel.transform.localPosition.x + 500, 0.8f).SetEase(Ease.InOutExpo);
        }
        _campVisibility = !_campVisibility;
    }

    public void SetupInfo(NewTileConfig config)
    {
        _button.enabled = true;
        _text.text = "Восстановите всем юнитам " + config.campSettings.healAmount.ToString() + " ед здоровья.";
        _button.onClick.AddListener(() => ApplyHeal(config.campSettings.healAmount));
    }

    public void ApplyHeal(int _amount)
    {
        if (ResourcesMNG.Instance.TryConsumeResource(ResourcesType.SignalTransmitter, 1))
        {
            _button.enabled = false;
        }
        else
        {
            Debug.Log("Недостаточно ресурса SignalTransmitter для лечения юнитов!");
        }
    }

}
