using System;
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
        delay = LeanTween.delayedCall(0.8f, () => 
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
        if (node.tileConfig == null) return "����������� �������";

        switch (node.tileConfig.tileType)
        {
            case TileType.BattleTile:
                return node.tileConfig.battleSettings?.tileEnemies?._enemiesName ?? "���";

            case TileType.BossTile:
                return node.tileConfig.bossSettings?.tileEnemies?._enemiesName ?? "����";

            case TileType.CampTile:
                return "������";

            case TileType.LootTile:
                return "������";

            case TileType.RouletteTile:
                return "�������";

            default:
                return node.tileConfig.tileType.ToString();
        }
    }

    private string GetLocalizedContent(MapNode node)
    {
        if (node.tileConfig == null) return "���������� �����������";

        switch (node.tileConfig.tileType)
        {
            case TileType.BattleTile:
                var battle = node.tileConfig.battleSettings;
                return $"������� ���������: {battle?.battleDifficulty ?? 0}\n" +
                       $"�������: {GetRewardDescription(battle?.reward)}";

            case TileType.BossTile:
                var boss = node.tileConfig.bossSettings;
                return $"����� � ������\n" +
                       $"���������: {boss?.battleDifficulty ?? 0}\n" +
                       $"�������: {GetRewardDescription(boss?.reward)}";

            case TileType.CampTile:
                var camp = node.tileConfig.campSettings;
                return $"����� ������\n" +
                       $"��������������: +{camp?.healAmount ?? 0} HP\n" +
                       $"��������: {(camp?.isUpgradeAvailable ?? false ? "��������" : "")}" +
                       $"{(camp?.isReinforceAvailable ?? false ? " ������������" : "")}";

            case TileType.LootTile:
                return $"�������: {GetRewardDescription(node.tileConfig.lootSettings?.reward)}";

            case TileType.RouletteTile:
                return "��������� ����� � �������";

            default:
                return "������ �������� �����������";
        }
    }

    private string GetRewardDescription(SerializableRewardConfig reward)
    {
        if (reward == null) return "������� �� ����������";

        List<string> rewards = new List<string>();

        // ��������� ��������, ���� ��� ����
        if (reward.items != null && reward.items.Count > 0)
        {
            rewards.Add("��������");
        }

        // ��������� ����, ���� �� ����
        if (reward.expAmount > 0)
        {
            rewards.Add("����");
        }

        // ��������� �������, ���� ��� ����
        if (reward.resources != null && reward.resources.Count > 0)
        {
            rewards.Add("�������");
        }

        // ��������� �������� ������
        switch (rewards.Count)
        {
            case 0:
                return "������ �������";
            case 1:
                return $"����������� �������� {rewards[0]}";
            case 2:
                return $"����������� �������� {rewards[0]} � {rewards[1]}";
            default:
                return $"����������� �������� {string.Join(", ", rewards.Take(rewards.Count - 1))} � {rewards.Last()}";
        }
    }


    public void SetItemTooltip(ItemInstance item)
    {

        Header = item.ItemName ?? "����������� �������";
        Content = BuildItemDescription(item);
    }

    private string BuildItemDescription(ItemInstance item)
    {
        StringBuilder sb = new StringBuilder();

        // �������� ��������������
        sb.AppendLine($"���: {GetActionTypeName(item.ActionType)}");
        string _powerline = item.Power > 0 ? "+" + item.Power: "-" + item.Power;
        sb.AppendLine($"����: {_powerline}");

        // ������ �������
        if (item.SideAffect != ItemSideAffect.None)
        {
            sb.AppendLine($"������ ��: {GetSideAffectDescription(item.SideAffect)}");
        }

        return sb.ToString();
    }

    public  string GetActionTypeName(ActionType type)
    {
        switch (type)
        {
            case ActionType.Attack: return "�����";
            case ActionType.Defense: return "������";
            case ActionType.Heal: return "�������";
            case ActionType.LifeSteal: return "����� ��������";
            case ActionType.Moral: return "������";
            case ActionType.HealthAttack: return "����� ���������";
            case ActionType.ShieldBash: return "���� �����";
            default: return "��� �������";
        }
    }

    private string GetSideAffectDescription(ItemSideAffect affect)
    {
        switch (affect)
        {
            case ItemSideAffect.Nearest: return "������� � ��� �� �������";
            case ItemSideAffect.Even: return "׸���� ������� => 2,4,6";
            case ItemSideAffect.Odd: return "�������� ������� => 1,3,5";
            case ItemSideAffect.All: return "��� ������� => 1,2,3,4,5,6";
            case ItemSideAffect.Touching: return "��������������� ������� => 1,2 | 2,3 | 3,4 | 1,4";
            default: return "����������� ����";
        }
    }
    public void SetBuffTooltip(BuffConfig buff)
    {
        Header = !string.IsNullOrEmpty(buff.buffName) ? buff.buffName : "����������� ����";
        Content = BuildBuffDescription(buff);
    }

    private string BuildBuffDescription(BuffConfig buff)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"����������� ���� ������: {GetActionTypeName(buff.buffType)}");
        sb.AppendLine($"����: +{buff.buffPower}");
        return sb.ToString();
    }

    public void SetUnitUpgradeTooltip(NewUnitStats unit)
    {
        if (unit == null)
        {
            Header = "����������� ����";
            Content = "���������� �����������";
            return;
        }

        Header = !string.IsNullOrEmpty(unit._name) ? unit._name : "���������� ����";
        Content = BuildUnitTooltipContent(unit);
    }

    public void SetUnitCurrentTooltip(NewUnitStats unit)
    {
        if (unit == null)
        {
            Header = "����������� ����";
            Content = "���������� �����������";
            return;
        }

        Header = !string.IsNullOrEmpty(unit._name) ? unit._name : "���������� ����";
        Content = BuildUnitTooltipContent(unit, true);
    }
    private string BuildUnitTooltipContent(NewUnitStats unit, bool showExp = false)
    {
        if (unit == null) return "���������� �����������";

        StringBuilder sb = new StringBuilder();

        AppendBaseStats(sb, unit, showExp);
        AppendDiceInfo(sb, unit);
        AppendBuffsInfo(sb, unit);
        AppendUpgradesInfo(sb, unit);
        sb.AppendLine($"<size=20>�������� ���������: {unit._level} ���������� ��������.");
        return sb.ToString();
    }

    private void AppendBaseStats(StringBuilder sb, NewUnitStats unit, bool showExp)
    {
        sb.AppendLine($"�������: {unit._level}");
        sb.AppendLine($"��������: {unit._current_health}/{unit._health}");
        sb.AppendLine($"������: {unit._currentMoral}");

        if (showExp)
        {
            sb.AppendLine($"����: {unit._current_exp}/{unit._level * 10}");
        }
    }

    private void AppendDiceInfo(StringBuilder sb, NewUnitStats unit)
    {
        sb.AppendLine("����� ����:");

        if (unit._dice == null || unit._dice._diceConfig?.sides == null) return;

        int sideNumber = 1;
        foreach (var side in unit._dice._diceConfig.sides)
        {
            sb.AppendLine($"{sideNumber++}. {GetFormattedSideInfo(side)}");
        }
    }

    private string GetFormattedSideInfo(DiceSide side)
    {
        return $"{GetActionTypeName(side.actionType)}: {side.power + side.bonus}";
    }

    private void AppendBuffsInfo(StringBuilder sb, NewUnitStats unit)
    {
        if (unit._buffs == null || unit._buffs.Count == 0) return;

        sb.AppendLine($"�����: {unit._buffs.Count}");

        int buffsToShow = Math.Min(3, unit._buffs.Count);
        for (int i = 0; i < buffsToShow; i++)
        {
            sb.AppendLine($"- {unit._buffs[i].buffName} (+{unit._buffs[i].buffPower})");
        }

        if (unit._buffs.Count > 3)
        {
            sb.AppendLine($"- � ��� {unit._buffs.Count - 3}...");
        }
    }

    private void AppendUpgradesInfo(StringBuilder sb, NewUnitStats unit)
    {
        sb.AppendLine();

        if (unit._upgrade_list == null || unit._upgrade_list.Count == 0)
        {
            sb.AppendLine("��� ��������� ���������");
            return;
        }

        sb.AppendLine($"�������� ���������: {unit._upgrade_list.Count}");
    }
    public void SetUnitBattleTooltip(NewUnitStats unit)
    {
        if (unit == null)
        {
            return;
        }

        Header = !string.IsNullOrEmpty(unit._name) ? unit._name : "���������� ����";
        Content = BuildBattleDiceContent(unit);
    }

    private string BuildBattleDiceContent(NewUnitStats unit)
    {
        StringBuilder sb = new StringBuilder();
        AppendDiceInfo(sb, unit);

        return sb.ToString();
    }



}
