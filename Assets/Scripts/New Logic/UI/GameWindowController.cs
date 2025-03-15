using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameWindowController : MonoBehaviour
{
    public static GameWindowController Instance { get; private set; }

    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _text;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        _button.onClick.AddListener(()=>CallPanel(-1));
        _button.onClick.AddListener(()=>UIController.Instance.GoBack());
    }
    
    public void CallPanel(int direction)
    {
        _panel.transform.DOLocalMoveY(_panel.transform.localPosition.y - 960*direction, 1f).SetEase(Ease.InOutExpo);
    }

    public void SetupRouletteInfo(SectorConfig _reward)
    {
        if (_reward == null || _reward.WinItems == null || _reward.WinItems.Count == 0)
        {
            _text.text = "<size=40><b>Нет наград</b></size>";
            return;
        }

        // Заголовок
        string formattedText = "<size=40><b>Ваши награды:</b></size>\n\n";

        foreach (var reward in _reward.WinItems)
        {
            // Добавляем ресурсы
            if (reward.resource != null && reward.resource.Count > 0)
            {
                formattedText += "<b>Ресурсы:</b>\n";
                foreach (var res in reward.resource)
                {
                    formattedText += $"- {res.Config.ResourceName}: {res.Count}\n";
                }
            }

            // Добавляем опыт
            if (reward.expAmount > 0)
            {
                formattedText += $"<b>Опыт:</b> {reward.expAmount}\n";
            }

            // Добавляем предмет
            if (reward.item != null)
            {
                formattedText += $"<b>Предмет:</b> {reward.item.itemName}\n";
            }

            formattedText += "\n"; // Добавляем отступ между наградами
        }

        _text.text = formattedText.TrimEnd();
    }


}
