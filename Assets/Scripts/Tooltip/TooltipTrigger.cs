using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static CanvasMapGenerator;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Multiline()]
    [SerializeField]private string content = "";
    [SerializeField]private string header = "";

    public string Content { get => content; set => content = value; }
    public string Header { get => header; set => header = value; }

    public static LTDescr delay;

    public void OnPointerEnter(PointerEventData eventData)
    {
        delay = LeanTween.delayedCall(0.5f, () => 
        {
            TooltipSystem.Show(Content, Header);
         });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(delay.uniqueId);
        TooltipSystem.Hide();
    }

    public void SetTooltipByNode(MapNode node)
    {
        Header = GetLocalizedHeader(node);
        Content = GetLocalizedContent(node);
    }
    private string GetLocalizedHeader(MapNode node)
    {
        if (node.tileConfig == null) return "Неизвестная локация";

        switch (node.tileConfig.tileType)
        {
            case TileType.BattleTile:
                return node.tileConfig.battleSettings?.tileEnemies?._enemiesName ?? "Бой";

            case TileType.BossTile:
                return node.tileConfig.bossSettings?.tileEnemies?._enemiesName ?? "Босс";

            case TileType.CampTile:
                return "Лагерь";

            case TileType.LootTile:
                return "Добыча";

            case TileType.RouletteTile:
                return "Рулетка";

            default:
                return node.tileConfig.tileType.ToString();
        }
    }

    private string GetLocalizedContent(MapNode node)
    {
        if (node.tileConfig == null) return "Информация отсутствует";

        switch (node.tileConfig.tileType)
        {
            case TileType.BattleTile:
                var battle = node.tileConfig.battleSettings;
                return $"Уровень сложности: {battle?.battleDifficulty ?? 0}\n" +
                       $"Награда: {GetRewardDescription(battle?.reward)}";

            case TileType.BossTile:
                var boss = node.tileConfig.bossSettings;
                return $"Битва с боссом\n" +
                       $"Сложность: {boss?.battleDifficulty ?? 0}\n" +
                       $"Награда: {GetRewardDescription(boss?.reward)}";

            case TileType.CampTile:
                var camp = node.tileConfig.campSettings;
                return $"Место отдыха\n" +
                       $"Восстановление: +{camp?.healAmount ?? 0} HP\n" +
                       $"Доступно: {(camp?.isUpgradeAvailable ?? false ? "Апгрейды" : "")}" +
                       $"{(camp?.isReinforceAvailable ?? false ? ", Подкрепление" : "")}";

            case TileType.LootTile:
                return $"Награда: {GetRewardDescription(node.tileConfig.lootSettings?.reward)}";

            case TileType.RouletteTile:
                return "Испытайте удачу в рулетке";

            default:
                return "Особые свойства отсутствуют";
        }
    }

    private string GetRewardDescription(SerializableRewardConfig reward)
    {
        if (reward == null) return "Награда не определена";

        List<string> rewards = new List<string>();

        // Добавляем предметы, если они есть
        if (reward.items != null && reward.items.Count > 0)
        {
            rewards.Add("предметы");
        }

        // Добавляем опыт, если он есть
        if (reward.expAmount > 0)
        {
            rewards.Add("опыт");
        }

        // Добавляем ресурсы, если они есть
        if (reward.resources != null && reward.resources.Count > 0)
        {
            rewards.Add("ресурсы");
        }

        // Формируем итоговую строку
        switch (rewards.Count)
        {
            case 0:
                return "Особая награда";
            case 1:
                return $"Возможность получить {rewards[0]}";
            case 2:
                return $"Возможность получить {rewards[0]} и {rewards[1]}";
            default:
                return $"Возможность получить {string.Join(", ", rewards.Take(rewards.Count - 1))} и {rewards.Last()}";
        }
    }


    public void SetItemTooltip(ItemInstance item)
    {

        Header = item.ItemName ?? "Неизвестный предмет";
        Content = BuildItemDescription(item);
    }

    private string BuildItemDescription(ItemInstance item)
    {
        StringBuilder sb = new StringBuilder();

        // Основные характеристики
        sb.AppendLine($"Тип: {GetActionTypeName(item.ActionType)}");
        string _powerline = item.Power > 0 ? "+" + item.Power: "-" + item.Power;
        sb.AppendLine($"Сила: {_powerline}");

        // Особые эффекты
        if (item.SideAffect != ItemSideAffect.None)
        {
            sb.AppendLine($"Влияет на: {GetSideAffectDescription(item.SideAffect)}");
        }

        return sb.ToString();
    }

    private string GetActionTypeName(ActionType type)
    {
        switch (type)
        {
            case ActionType.Attack: return "Атака";
            case ActionType.Defense: return "Защита";
            case ActionType.Heal: return "Лечение";
            case ActionType.LifeSteal: return "Кража здоровья";
            case ActionType.Moral: return "Мораль";
            case ActionType.HP: return "Здоровье";
            default: return "Без эффекта";
        }
    }

    private string GetSideAffectDescription(ItemSideAffect affect)
    {
        switch (affect)
        {
            case ItemSideAffect.Nearest: return "Ближайшая по-часовой => номер стороны";
            case ItemSideAffect.Even: return "Чётные стороны => 2,4,6";
            case ItemSideAffect.Odd: return "Нечётные стороны => 1,3,5";
            case ItemSideAffect.All: return "Все стороны => 1,2,3,4,5,6";
            case ItemSideAffect.Touching: return "Соприкасающиеся стороны => номер стороны, номер стороны + 1";
            default: return "Неизвестная зона";
        }
    }
    public void SetBuffTooltip(BuffConfig buff)
    {
        Header = !string.IsNullOrEmpty(buff.buffName) ? buff.buffName : "Неизвестный бафф";
        Content = BuildBuffDescription(buff);
    }

    private string BuildBuffDescription(BuffConfig buff)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"Тип эффекта: {GetBuffTypeName(buff.buffType)}");
        sb.AppendLine($"Сила: {buff.buffPower}");

        switch (buff.buffType)
        {
            case ActionType.Attack:
                sb.AppendLine("Увеличивает силу атаки всех сторон");
                break;
            case ActionType.Defense:
                sb.AppendLine("Увеличивает защиту получаемую защиту от всех сторон");
                break;
            case ActionType.Heal:
                sb.AppendLine("Увеличивает восстановление здоровье от всех сторон");
                break;
            case ActionType.LifeSteal:
                sb.AppendLine($"Увеличивает урон и кражу здоровья всех сторон");
                break;
        }

        return sb.ToString();
    }

    private string GetBuffTypeName(ActionType type)
    {
        switch (type)
        {
            case ActionType.Attack: return "Атака";
            case ActionType.Defense: return "Защита";
            case ActionType.Heal: return "Лечение";
            case ActionType.LifeSteal: return "Кража здоровья";
            case ActionType.Moral: return "Мораль";
            case ActionType.HP: return "Здоровье";
            default: return "Бонус";
        }
    }
}
