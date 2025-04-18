using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArsenalPanelUI : MonoBehaviour
{
    private void Start()
    {
        UnitsPanelUI.SceneLoaded += SetupPlayerInvenory;
        UnitsPanelUI.OnUnitSelected += SetupUnitInventory;
    }

    [SerializeField] private GameObject _inventorySlot;
    [SerializeField] private GameObject _contentPlaceHolder;
    [SerializeField] private DraggableItem _item;
    [SerializeField] private Canvas _canvas;
    public void SetupPlayerInvenory()
    {
        ClearInventory();
        foreach (var item in GameDataMNG.Instance.PlayerData.Items)
        {
            if (item != null)
            {
                GameObject slotGO = Instantiate(_inventorySlot, _contentPlaceHolder.transform);
                if (item.InventoryPosition == -1)
                {
                    CreateItem(item, slotGO);
                }
            }
        }
    }

    public void SetupUnitInventory(NewUnitStats unit)
    {
        ClearActiveSlots();
        foreach (var item in unit._dice._items)
            foreach (var slot in ItemMNG.Instance.ActiveSlotForItems)
            {
                if (item.InventoryPosition == slot.InventoryPosition)
                {
                    CreateItem(item, slot.gameObject);
                    break;
                }
            }
    }

    private void CreateItem(ItemInstance item, GameObject slotGO)
    {
        DraggableItem draggable = Instantiate(_item, slotGO.transform);
        if (draggable != null)
        {
            draggable.Setup(item, _canvas);
        }
    }

    private void ClearInventory()
    {
        foreach (Transform child in _contentPlaceHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void ClearActiveSlots()
    {
        foreach (var slot in ItemMNG.Instance.ActiveSlotForItems)
        {
            foreach (Transform child in slot.gameObject.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }


}
