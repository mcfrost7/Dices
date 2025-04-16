using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMNG : MonoBehaviour
{
    public static ItemMNG Instance { get; private set; }
    public List<InventorySlot> ActiveSlotForItems { get => _activeItems; set => _activeItems = value; }

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
    [SerializeField] private List<InventorySlot> _activeItems;

    public void AddItem(ItemInstance item)
    {
        GameDataMNG.Instance.PlayerData.Items.Add(item);
    }


}
