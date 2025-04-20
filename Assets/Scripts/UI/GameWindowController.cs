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
        Button.onClick.AddListener(() => CallPanel(-1));
    }

    public void CallPanel(int _direction)
    {
        _panel.transform.DOLocalMoveY(_panel.transform.localPosition.y - 960 * _direction, 1f).SetEase(Ease.InOutExpo);
        MenuMNG.Instance.CallFreezePanel(_direction);
    }

    public void SetupRouletteInfo(SectorConfig rewardConfig)
    {
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(() => CallPanel(-1));

        if (rewardConfig == null || rewardConfig.WinItems == null || rewardConfig.WinItems.ItemInstance == null)
        {
            _title.text = "<color=#E31837><size=42><b>ПУСТЫЕ ТРОФЕИ</b></size></color>";
            _text.text = "<size=36><color=#8A8A8A><i>Ничего не найдено в руинах...</i></color></size>";
            Button.onClick.AddListener(() => GlobalWindowController.Instance.GoBack());
            return;
        }

        _title.text = "<color=#8B0000><size=42><b>ДОБЫЧА ВОЙНЫ</b></size></color>";

        StringBuilder lootText = new StringBuilder();
        lootText.AppendLine("<color=#5A5A5A>-----------------------------------------------------------------</color>");
        lootText.AppendLine();

        lootText.Append(FormatReward(rewardConfig.WinItems));

        lootText.AppendLine("<color=#5A5A5A>-----------------------------------------------------------------</color>");
        lootText.AppendLine("<size=32><color=#8B0000><i>«ДОБЫЧА ДОСТАНЕТСЯ СИЛЬНЕЙШЕМУ»</i></color></size>");

        _text.text = lootText.ToString();
        Button.onClick.AddListener(() => GlobalWindowController.Instance.GoBack());
    }

    public void SetupWinBattleInfo(NewTileConfig tile)
    {
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(() => CallPanel(-1));
        Button.onClick.AddListener(() => GlobalWindowController.Instance.ShowGlobalCanvas());
        Button.onClick.AddListener(() => BattleUI.Instance.HideVictoryScreen());

        if (tile == null)
        {
            _title.text = "<color=#8B0000><size=42><b>НЕТ ДОСТОЙНОЙ ЦЕЛИ</b></size></color>";
            _text.text = "<size=36><color=#5A5A5A><i>Победа без славы — как пир без мяса.</i></color></size>";
            return;
        }

        _title.text = "<color=#8B0000><size=42><b>ВО ИМЯ ИМПЕРАТОРА!</b></size></color>";
        SerializableRewardConfig rewardConfig = !BattleController.Instance.IsBossBattle ? tile.battleSettings.reward : tile.bossSettings.reward;

        StringBuilder victoryText = new StringBuilder();
        victoryText.AppendLine("<color=#5A5A5A>-----------------------------------------------------------------</color>");
        victoryText.AppendLine();
        victoryText.Append(FormatReward(rewardConfig));
        victoryText.AppendLine("<color=#5A5A5A>-----------------------------------------------------------------</color>");
        victoryText.AppendLine("<size=32><color=#8B0000><i>«ПОБЕДА — ЕДИНСТВЕННЫЙ ЗАКОН ВОЙНЫ»</i></color></size>");

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
                     "<size=26><color=#8B0000><i>«В ВОЙНЕ НЕТ ПОБЕД — ЕСТЬ ТОЛЬКО ВЫЖИВАНИЕ. ВЫ НЕ ВЫЖИЛИ.»</i></color></size>";
        Button.onClick.AddListener(() => BattleUI.Instance.HideDefeatScreen());
        Button.onClick.AddListener(() => BattleController.Instance.OnBattleLose());
    }

    public void SetupResourceInfo(SerializableRewardConfig reward)
    {
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(() => CallPanel(-1));

        _title.text = "<color=#8B0000><size=42><b>РЕСУРСЫ ДОБЫТЫ</b></size></color>";

        StringBuilder resourceText = new StringBuilder();
        resourceText.AppendLine("<color=#5A5A5A>-----------------------------------------------------------------</color>");
        resourceText.AppendLine();
        resourceText.Append(FormatReward(reward));
        resourceText.AppendLine("<color=#5A5A5A>-----------------------------------------------------------------</color>");
        resourceText.AppendLine("<size=32><color=#8B0000><i>«ВОЙНА КОРМИТ ВОЙНУ»</i></color></size>");

        _text.text = resourceText.ToString();
    }

    private string FormatReward(SerializableRewardConfig reward)
    {
        StringBuilder sb = new StringBuilder();

        if (reward.expAmount > 0)
        {
            sb.AppendLine("<color=#8B4513><size=32><b>СЛАВА</b></size></color>");  // Тёмно-коричневый
            sb.AppendLine($"{reward.expAmount} ед. опыта\n");
        }

        if (reward.resources != null && reward.resources.Count > 0)
        {
            sb.AppendLine("<color=#556B2F><size=32><b>ТРОФЕИ</b></size></color>");  // Оливковый
            foreach (var res in reward.resources)
            {
                sb.AppendLine($"{res.Config.ResourceName.ToUpper()}: {res.Count}");
            }
            sb.AppendLine();
        }

        if (reward.ItemInstance != null && reward.items.Count > 0)
        {
            sb.AppendLine("<color=#6B8E23><size=32><b>РЕЛИКВИИ</b></size></color>");  // Защитный
            sb.AppendLine($"{reward.ItemInstance.ItemName}\n");
        }

        return sb.ToString();
    }

    public void SetupCampfireTileInfo(NewTileConfig _tile)
    {
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(() => CallPanel(-1));

        _title.text = "<size=42><color=#8B0000>УКРЫТИЕ</color></size>";

        _text.text = "<size=30>" +
                     "<color=#5A5A5A>-------------------------------------</color>\n" +
                     "<color=#2F2F2F>В ЗАБРОШЕННЫХ ОТСЕКАХ ДРЕВНЕГО КОРАБЛЯ БОЙЦЫ НАШЛИ ВРЕМЕННОЕ УБЕЖИЩЕ." +
                     "СРЕДИ ГУЛА ДРЕМЛЮЩИХ МАШИН И МЕРЦАЮЩИХ РУН МОЖНО:\n\n" +
                     "<color=#8B0000>▪</color> <color=#B8860B>ПРИМЕНИТЬ АПТЕКАРИСКЕИ АМПЛАНТЫ</color> - ВОССТАНОВИТЬ ЗДОРОВЬЕ\n" +
                     "<color=#8B0000>▪</color> <color=#B8860B>ИЗУЧИТЬ АРХИВЫ</color> - УЛУЧШИТЬ ЭКИПИРОВКУ\n" +
                     "<color=#8B0000>▪</color> <color=#B8860B>СВЯЗАТЬСЯ С КОМАНДОВАНИЕМ</color> - ПОПОЛНИТЬ РЯДЫ\n" +
                     "<color=#5A5A5A>-------------------------------------</color>\n" +
                     "<size=32><color=#8B0000><i>«ДАЖЕ В АДУ НАЙДЁТСЯ МЕСТО ДЛЯ ПЕРЕДЫШКИ»</i></color></size>" +
                     "</size>";
    }
}