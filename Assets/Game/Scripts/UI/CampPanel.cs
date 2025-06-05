using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class CampPanel : MonoBehaviour
{
    public static CampPanel Instance { get; private set; }
    public Button Button { get => _button; set => _button = value; }
    public bool CampVisibility { get => _campVisibility; set => _campVisibility = value; }

    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _button;
    [SerializeField] private Button _buttonCallHealPanel;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _textName;

    private bool _campVisibility = false;
    private NewTileConfig _config;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _button.onClick.AddListener(() => ApplyHeal());
        MenuMNG.Instance.AddButtonListener(_buttonCallHealPanel, ToggleCampVisibility);
        CallButtonActivity(false);

    }

    private bool _isAnimating = false;
    public void ToggleCampVisibility()
    {
        SetCampVisibility(!CampVisibility);
    }

    public void SetCampVisibility(bool isVisible)
    {
        if (_isAnimating || CampVisibility == isVisible) return;

        _isAnimating = true;

        float targetPositionX = isVisible
            ? _panel.transform.localPosition.x - 500
            : _panel.transform.localPosition.x + 500;

        _panel.transform.DOLocalMoveX(targetPositionX, 0.8f)
            .SetEase(Ease.InOutExpo)
            .OnComplete(() => _isAnimating = false);

        CampVisibility = isVisible;
    }

    public void SetupInfo(NewTileConfig config)
    {
        _config = config;
        _button.enabled = true;
        _textName.text = "<color=#8B0000>УКРЫТИИ</color>";
        _text.text = $"<color=#5A5A5A>-----------------------------</color>\n" +
                     $"<color=#2F2F2F>Использовать полевые аптечки для восстановления <color=#B8860B>{config.campSettings.healAmount}</color> ед. здоровья каждому воину.</color>\n" +
                     $"<color=#5A5A5A>-----------------------------</color>\n" +
                     $"<color=#2F2F2F>Требуется: <color=#8B0000>1</color> передатчик сигнала.</color>";

    }

    public void ApplyHeal()
    {
        if (ResourcesMNG.Instance.TryConsumeResource(ResourcesType.SignalTransmitter, 1))
        {
            _button.enabled = false;
            foreach (var unit in GameDataMNG.Instance.PlayerData.PlayerUnits)
            {
                unit._current_health = Mathf.Min(unit._current_health + _config.campSettings.healAmount, unit._health);
            }
            SFXManager.Instance.PlaySoundSM(ActionType.Heal);
            _text.text = $"<color=#5A5A5A>-----------------------------</color>\n" +
                         $"<color=#2F2F2F>Здоровье пополнено. Отряд готов к продолжению операции.</color>\n" +
                         $"<color=#5A5A5A>-----------------------------</color>";
        }
        else
        {
            _text.text = $"<color=#5A5A5A>-----------------------------</color>\n" +
                         $"<color=#8B0000><b>ОТКАЗ</b></color>\n" +
                         $"<color=#2F2F2F>Передатчик сигнала не обнаружен в снаряжении!</color>\n" +
                         $"<color=#5A5A5A>-----------------------------</color>\n" +
                         $"<color=#2F2F2F>Требуется: <color=#8B0000>1</color> передатчик сигнала.</color>";
        }
    }

    public void CallButtonActivity(bool _state)
    {
        _buttonCallHealPanel.gameObject.SetActive(_state);
        _campVisibility = !_state;
    }
}
