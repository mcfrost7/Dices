using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static CanvasMapGenerator;

public class TileLogic : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TooltipTrigger _tooltipTrigger;

    public Image Image { get => _image; set => _image = value; }
    public Button Button { get => _button; set => _button = value; }
    public TextMeshProUGUI Text { get => _text; set => _text = value; }

    private UnityAction<MapNode> _clickCallback; 
    private MapNode _nodeData; 

    public void Initialize(Sprite sprite, UnityAction<MapNode> clickCallback, MapNode nodeData)
    {
        Image.sprite = sprite;
        _clickCallback = clickCallback;
        _nodeData = nodeData;
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(OnButtonClick);
        _tooltipTrigger.SetTooltipByNode(nodeData);
    }

    private void OnButtonClick()
    {
        _clickCallback?.Invoke(_nodeData);
    }
}
