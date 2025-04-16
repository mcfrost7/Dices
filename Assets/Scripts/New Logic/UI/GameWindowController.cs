using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameWindowController : MonoBehaviour
{
    public static GameWindowController Instance { get; private set; }
    public Button Button { get => _button; set => _button = value; }

    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _title;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Button.onClick.AddListener(()=>CallPanel(-1));
    }
    
    public void CallPanel(int _direction)
    {
        _panel.transform.DOLocalMoveY(_panel.transform.localPosition.y - 960*_direction, 1f).SetEase(Ease.InOutExpo);
        MenuMNG.Instance.CallFreezePanel(_direction);
    }

    public void SetupRouletteInfo(SectorConfig _rewardConfig)
    {
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(() => CallPanel(-1));
        if (_rewardConfig == null || _rewardConfig.WinItems == null || _rewardConfig.WinItems.Count == 0)
        {
            _title.text = "<color=#E31837><b>ПУСТЫЕ ТРОФЕИ</b></color>";
            _text.text = "<size=36><color=#8A8A8A><i>Ничего не найдено в руинах...</i></color></size>";
            return;
        }

        _title.text = "<color=#8B0000><b>ДОБЫЧА ВОЙНЫ</b></color>";

        StringBuilder lootText = new StringBuilder();
        lootText.AppendLine("<color=#5A5A5A>-----------------------------</color>");
        lootText.AppendLine();

        foreach (var reward in _rewardConfig.WinItems)
        {
            if (reward.resource != null && reward.resource.Count > 0)
            {
                lootText.AppendLine("<color=#2F2F2F><b>ТРОФЕИ:</b></color>");
                foreach (var res in reward.resource)
                {
                    lootText.AppendLine($"<color=#8B0000></color> {res.Config.ResourceName.ToUpper()}: <color=#B8860B>{res.Count}</color>");
                }
            }

            if (reward.expAmount > 0)
            {
                lootText.AppendLine($"<color=#B8860B><b>СЛАВА:</b></color> +{reward.expAmount}");
            }

            if (reward.item != null)
            {
                lootText.AppendLine("<color=#2F2F2F><b>РЕЛИКВИЯ:</b></color>");
                lootText.AppendLine($"<color=#8B0000></color> <b>{reward.item.itemName.ToUpper()}</b>");
            }
        }

        lootText.AppendLine("<color=#5A5A5A>-----------------------------</color>");
        lootText.AppendLine("<size=32><color=#8B0000>«ДАЖЕ ПЕПЕЛ ВРАГА ПРИГОДИТСЯ»</color></size>");

        _text.text = lootText.ToString();
        Button.onClick.AddListener(()=>GlobalWindowController.Instance.GoBack());
    }

    public void SetupWinBattleInfo(NewTileConfig _tile)
    {
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(() => CallPanel(-1));

        if (_tile == null)
        {
            _title.text = "<color=#8B0000><b>НЕТ ДОСТОЙНОЙ ЦЕЛИ</b></color>";
            _text.text = "<size=36><color=#5A5A5A><i>Победа без славы — как пир без мяса.</i></color></size>";
            return;
        }

        // Заголовок (оптимизированные цвета)
        _title.text = "<color=#8B0000><b>ВО ИМЯ ИМПЕРАТОРА!</b></color>";

        StringBuilder victoryText = new StringBuilder();

        // Разделитель (тёмно-серый)
        victoryText.AppendLine("<color=#5A5A5A>-----------------------------------------------------------------</color>");
        victoryText.AppendLine();

        RewardConfig _rewardConfig = !BattleController.Instance.IsBossBattle
            ? _tile.battleSettings.reward
            : _tile.bossSettings.reward;

        // Опыт (тёмное золото)
        victoryText.AppendLine($"<color=#B8860B><b>СЛАВА:</b></color> +{_rewardConfig.expAmount} ед. опыта");
        victoryText.AppendLine();

        // Ресурсы (тёмно-красные акценты)
        if (_rewardConfig.resource != null && _rewardConfig.resource.Count > 0)
        {
            victoryText.AppendLine("<color=#5A5A5A><b>ТРОФЕИ:</b></color>");
            foreach (var res in _rewardConfig.resource)
            {
                victoryText.AppendLine($"<color=#8B0000></color> {res.Config.ResourceName.ToUpper()}: <color=#B8860B>{res.Count}</color>");
            }
            victoryText.AppendLine();
        }

        // Предмет (тёмно-красный акцент)
        if (_rewardConfig.item != null)
        {
            victoryText.AppendLine("<color=#5A5A5A><b>РЕЛИКВИЯ:</b></color>");
            victoryText.AppendLine($"<color=#8B0000></color> <b>{_rewardConfig.item.itemName.ToUpper()}</b>");
            victoryText.AppendLine();
        }

        // Заключительная строка
        victoryText.AppendLine("<color=#5A5A5A>-----------------------------------------------------------------</color>");
        victoryText.AppendLine("<size=36><color=#8B0000>«КРОВЬ ВРАГОВ — ЛУЧШАЯ НАГРАДА»</color></size>");

        _text.text = victoryText.ToString();
    }


    public void SetupDefeatBattleInfo()
    {
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(() => CallPanel(-1));
        _title.text = "<size=42><color=#8B0000>КАТАСТРОФА</color></size>";

        _text.text = "<size=30>" +
                     "<color=#5A5A5A>-------------------------------------</color>\n\n" +
                     "<color=#2F2F2F>ПОСЛЕДНИЙ ПЕРЕДАТЧИК ЗАМОЛК. ПОСЛЕДНИЙ БОЕЦ ПАЛ.\n\n" +
                     "ВАШ ОТРЯД УНИЧТОЖЕН.\n\n" +
                     "ЭТА ВОЙНА ПРОИГРАНА. ВАШИ ИМЕНА БУДУТ ЗАБЫТЫ.\n\n" +
                     "<color=#5A5A5A>-------------------------------------</color>\n\n" +
                     "<size=26><color=#8B0000><i>«В ВОЙНЕ НЕТ ПОБЕД — ЕСТЬ ТОЛЬКО ВЫЖИВАНИЕ. ВЫ НЕ ВЫЖИЛИ.»</i></color></size>" +
                     "</size>";
        Button.onClick.AddListener(() => BattleController.Instance.OnBattleLose());
    }

    public void SetupResourceInfo(RewardConfig reward)
    {
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(() => CallPanel(-1));
        // Заголовок в стиле Warhammer
        _title.text = "<color=#8B0000><b>РЕСУРСЫ ДОБЫТЫ</b></color>";

        StringBuilder resourceText = new StringBuilder();

        // Разделитель
        resourceText.AppendLine("<color=#5A5A5A>-------------------------------------</color>");
        resourceText.AppendLine();

        // Основной текст
        if (reward.resource != null && reward.resource.Count > 0)
        {
            resourceText.AppendLine("<color=#5A5A5A><b>ТРОФЕИ ВОЙНЫ:</b></color>");
            resourceText.AppendLine();

            foreach (var res in reward.resource)
            {
                resourceText.AppendLine($"<color=#8B0000></color> <color=#2F2F2F>{res.Config.ResourceName.ToUpper()}:</color> <color=#B8860B>{res.Count}</color>");
            }

            if (reward.expAmount > 0)
            {
                resourceText.AppendLine($"<color=#B8860B><b>СЛАВА:</b></color> +{reward.expAmount}");
            }

            if (reward.item != null)
            {
                resourceText.AppendLine("<color=#2F2F2F><b>РЕЛИКВИЯ:</b></color>");
                resourceText.AppendLine($"<color=#8B0000></color> <b>{reward.item.itemName.ToUpper()}</b>");
            }
        }
        else
        {
            resourceText.AppendLine("<color=#5A5A5A><i>Пусто... лишь пыль и пепел</i></color>");
        }

        // Заключительная строка
        resourceText.AppendLine();
        resourceText.AppendLine("<color=#5A5A5A>-------------------------------------</color>");
        resourceText.AppendLine("<size=32><color=#8B0000>«ВОЙНА КОРМИТ ВОЙНУ»</color></size>");

        _text.text = resourceText.ToString();
    }

    public void SetupCampfireTileInfo(NewTileConfig _tile)
    {
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(() => CallPanel(-1));
        _title.text = "<size=36><color=#8B0000>ПЕРЕДЫШКА В УКРЫТИИ</color></size>";

        _text.text = "<size=28>" +
                     "<color=#5A5A5A>-------------------------------------</color>\n" +
                     "<color=#2F2F2F>В ЗАБРОШЕННЫХ ОТСЕКАХ ДРЕВНЕГО КОРАБЛЯ БОЙЦЫ НАШЛИ ВРЕМЕННОЕ УБЕЖИЩЕ." +
                     "СРЕДИ ГУЛА ДРЕМЛЮЩИХ МАШИН И МЕРЦАЮЩИХ РУН МОЖНО:\n\n" +
                     "<color=#8B0000>▪</color> <color=#B8860B>ОТДОХНУТЬ</color> - ВОССТАНОВИТЬ ЗДОРОВЬЕ\n" +
                     "<color=#8B0000>▪</color> <color=#B8860B>ИЗУЧИТЬ АРХИВЫ</color> - УЛУЧШИТЬ ЭКИПИРОВКУ\n" +
                     "<color=#8B0000>▪</color> <color=#B8860B>СВЯЗАТЬСЯ С КОМАНДОВАНИЕМ</color> - ПОПОЛНИТЬ РЯДЫ\n" +
                     "<color=#5A5A5A>-------------------------------------</color>\n" +
                     "<size=32><color=#8B0000><i>«ДАЖЕ В АДУ НАЙДЁТСЯ МЕСТО ДЛЯ ПЕРЕДЫШКИ»</i></color></size>" +
                     "</size>";
    }
}
