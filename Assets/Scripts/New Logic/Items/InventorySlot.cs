using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{

    [SerializeField] private int _inventoryPosition = -1;
    private bool _isActive = false;

    public int InventoryPosition { get => _inventoryPosition; set => _inventoryPosition = value; }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        if (draggedItem == null) return;

        if (transform.childCount == 0)
        {
            draggedItem.parentAfterDrag = transform;

            if (draggedItem.Item != null)
            {
                draggedItem.Item.inventoryPosition = InventoryPosition;
                if (draggedItem.Item.inventoryPosition > 0)
                { 
                    foreach (var item in GameDataMNG.Instance.PlayerData.Items)
                    {
                        if (item == draggedItem.Item)
                        {
                            item.inventoryPosition = InventoryPosition;
                            break;
                        }
                    }
                    if (!UnitsPanelUI.Instance.CurrentUnit._dice._items.Contains(draggedItem.Item))
                    {
                        UnitsPanelUI.Instance.CurrentUnit._dice._items.Add(draggedItem.Item);
                    }
                    Debug.Log($"Предмет перемещен в слот {InventoryPosition}");
                }else
                {
                    foreach (var item in GameDataMNG.Instance.PlayerData.Items)
                    {
                        if (item == draggedItem.Item)
                        {
                            item.inventoryPosition = InventoryPosition;
                            break;
                        }
                    }
                    if (UnitsPanelUI.Instance.CurrentUnit._dice._items.Contains(draggedItem.Item))
                    {
                        UnitsPanelUI.Instance.CurrentUnit._dice._items.Remove(draggedItem.Item);
                    }
                }
            }
        }
    }

    public void OnItemTaken()
    {
        Debug.Log($"Предмет забрали из слота {InventoryPosition}");
    }
}