using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{

    [SerializeField] private int _inventoryPosition = -1;
    private bool _isActive = false;
    private DraggableItem draggedItem = null;
    public int InventoryPosition { get => _inventoryPosition; set => _inventoryPosition = value; }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;
        draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        if (draggedItem == null || draggedItem.Item == null) return;

        if (GameDataMNG.Instance == null || GameDataMNG.Instance.PlayerData == null ||
            UnitsPanelUI.Instance == null || UnitsPanelUI.Instance.CurrentUnit == null ||
            UnitsPanelUI.Instance.CurrentUnit._dice == null)
        {
            return;
        }

        if (transform.childCount == 0)
        {
            draggedItem.parentAfterDrag = transform;
            draggedItem.Item.InventoryPosition = InventoryPosition;
            var items = GameDataMNG.Instance.PlayerData.Items;
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item == draggedItem.Item)
                    {
                        item.InventoryPosition = InventoryPosition;
                        break;
                    }
                }
            }

            var diceItems = UnitsPanelUI.Instance.CurrentUnit._dice._items;
            if (InventoryPosition > 0)
            {
                if (!diceItems.Contains(draggedItem.Item))
                {
                    diceItems.Add(draggedItem.Item);
                }
            }
            else
            {
                if (diceItems.Contains(draggedItem.Item))
                {
                    diceItems.Remove(draggedItem.Item);
                }
            }
            GameDataMNG.Instance.SaveGame();
            EquipmentUI.Instance.SetupInfo(UnitsPanelUI.Instance.CurrentUnit);
        }
    }

    public void OnItemTaken()
    {
        Debug.Log($"Предмет забрали из слота {InventoryPosition}");
    }
}