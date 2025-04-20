using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Image _icon;

    [SerializeField] private RectTransform rectTransform; 
    [HideInInspector] public Transform parentAfterDrag;
    [SerializeField] public TooltipTrigger _trigger;


    private ItemInstance _item = null;
    private Canvas canvasParent;
    public ItemInstance Item { get => _item; set => _item = value; }


    public void Setup(ItemInstance item, Canvas canvas)
    {
        Item = item;
        _icon.sprite = item.Icon;
        canvasParent = canvas;
        _trigger.SetItemTooltip(item);
    }

    private void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(canvasParent.transform, true); // Используем canvas как временного родителя
        transform.SetAsLastSibling(); // Делаем объект поверх других
        _icon.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasParent.transform as RectTransform,
            eventData.position,
            canvasParent.worldCamera,
            out Vector2 localPoint);

        rectTransform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        _icon.raycastTarget = true;
    }
}