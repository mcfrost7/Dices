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
    public bool CampVisibility { get => _campVisibility; set => _campVisibility = value; }

    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _button;
    [SerializeField] private Button _buttonCallHealPanel;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _textName;

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
       _buttonCallHealPanel.onClick.AddListener(()=>CallTask());
       _buttonCallHealPanel.gameObject.SetActive(false);
    }

    private bool _isAnimating = false;
    public void CallTask()
    {
        if (_isAnimating) return;

        _isAnimating = true;

        if (CampVisibility == false)
        {
            _panel.transform.DOLocalMoveX(_panel.transform.localPosition.x - 500, 0.8f)
                .SetEase(Ease.InOutExpo)
                .OnComplete(() => _isAnimating = false);
        }
        else
        {
            _panel.transform.DOLocalMoveX(_panel.transform.localPosition.x + 500, 0.8f)
                .SetEase(Ease.InOutExpo)
                .OnComplete(() => _isAnimating = false);
        }
        CampVisibility = !CampVisibility;
    }

    public void SetupInfo(NewTileConfig config)
    {
        _button.enabled = true;
        _textName.text = "<color=#8B0000>�������</color>";
        _text.text = $"<color=#5A5A5A>-----------------------------</color>\n" +
                     $"<color=#2F2F2F>������������ ������� ������� ��� �������������� <color=#B8860B>{config.campSettings.healAmount}</color> ��. �������� ������� �����.</color>\n" +
                     $"<color=#5A5A5A>-----------------------------</color>\n" +
                     $"<color=#2F2F2F>���������: <color=#8B0000>1</color> ���������� �������.</color>";

        _button.onClick.AddListener(() => ApplyHeal(config.campSettings.healAmount));
    }

    public void ApplyHeal(int _amount)
    {
        if (ResourcesMNG.Instance.TryConsumeResource(ResourcesType.SignalTransmitter, 1))
        {
            _button.enabled = false;
            HealAction healAction = new HealAction();
            foreach (var unit in GameDataMNG.Instance.PlayerData.PlayerUnits)
            {
                healAction.Heal(unit, _amount);
            }
            SFXManager.Instance.PlaySound(ActionType.Heal);
            _text.text = $"<color=#5A5A5A>-----------------------------</color>\n" +
                         $"<color=#2F2F2F>�������� ���������. ����� ����� � ����������� ��������.</color>\n" +
                         $"<color=#5A5A5A>-----------------------------</color>";
        }
        else
        {
            _text.text = $"<color=#5A5A5A>-----------------------------</color>\n" +
                         $"<color=#8B0000><b>�����</b></color>\n" +
                         $"<color=#2F2F2F>���������� ������� �� ��������� � ����������!</color>\n" +
                         $"<color=#5A5A5A>-----------------------------</color>\n" +
                         $"<color=#2F2F2F>���������: <color=#8B0000>1</color> ���������� �������.</color>";
        }
    }
}
