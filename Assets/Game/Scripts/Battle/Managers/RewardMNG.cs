using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class RewardMNG : MonoBehaviour
{
    [SerializeField] private float _dropChance;
   public static RewardMNG Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public SerializableRewardConfig CalculateReward(SerializableRewardConfig reward)
    {
        // Create a new reward config that will contain only what was actually given
        SerializableRewardConfig actualReward = new SerializableRewardConfig(reward.SourceConfig);
        // Process rewards
        ResourceReward(actualReward.resources);
        ExpReward(actualReward.expAmount);

        ItemInstance droppedItem = ItemReward(reward);
        if (droppedItem != null)
        {
            actualReward.ItemInstance = droppedItem;
        }
        else if (reward.GetRandomItem() != null)
        {
            actualReward.PotentialItemLost = true;
        }

        return actualReward;
    }

    private void ResourceReward(List<ResourceData> resource)
    {
        if (resource.Count > 0 && resource != null)
        {
            ResourcesMNG.Instance.AddResources(resource);
        }
    }
    private void ExpReward(int exp)
    {
        if (exp > 0)
        {
            foreach (var unit in GameDataMNG.Instance.PlayerData.PlayerUnits)
            {
                if (unit == null) continue;

                int maxExp = unit._level * 10;
                unit._current_exp = Mathf.Min(unit._current_exp + exp, maxExp);
            }
            string notificationText = GetLevelUpNotificationText(GameDataMNG.Instance.PlayerData.PlayerUnits);
            if (!string.IsNullOrEmpty(notificationText))
            {
                MenuMNG.Instance.SetupLevelNotification(notificationText);
            }
        }
    }
    private ItemInstance ItemReward(SerializableRewardConfig reward)
    {
        if (reward == null) return null;

        ItemInstance item = reward.GetRandomItem();
        if (item != null && item.Config != null)
        {
            if (Random.Range(0f, 100f) <= _dropChance)
            {
                ItemMNG.Instance.AddItem(item);
                return item;
            }
        }

        return null;
    }

    private string GetLevelUpNotificationText(List<NewUnitStats> units)
    {
        StringBuilder text = new StringBuilder();
        List<string> leveledUpUnitNames = new List<string>();

        foreach (var unit in units)
        {
            if (unit == null) continue;

            int maxExp = unit._level * 10;
            if (unit._current_exp >= maxExp)
            {
                leveledUpUnitNames.Add($"<color=#FF5555>{unit._name.ToUpperInvariant()}</color>");
            }
        }

        if (leveledUpUnitNames.Count == 0)
            return string.Empty;

        text.AppendLine("<size=30><color=#8B0000>УРОВЕНЬ ДОСТИГНУТ</color></size>");
        text.AppendLine("<color=#5A5A5A>-------------------------------------</color>");
        text.Append("<size=20>");
        text.AppendLine(string.Join("<color=#8B0000> ▪ </color>", leveledUpUnitNames));
        text.AppendLine("<color=#2F2F2F>эти герои закалены в боях и обрели весь доступный опыт.</color>");
        text.AppendLine("<color=#B8860B>в ближайшем <color=#00FFFF>лагере</color> они могут быть улучшены.</color>");
        text.AppendLine();
        text.AppendLine("<size=28><color=#8B0000><i>«империя требует совершенства»</i></color></size>");
        text.Append("</size>");

        return text.ToString();
    }



}
