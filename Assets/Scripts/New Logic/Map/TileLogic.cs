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

    public Image Image { get => _image; set => _image = value; }
    public Button Button { get => _button; set => _button = value; }
    public TextMeshProUGUI Text { get => _text; set => _text = value; }

    private UnityAction<MapNode> _clickCallback;  // Сюда передаем событие клика
    private MapNode _nodeData;  // Данные о текущем узле

    // Инициализация кнопки и изображения
    public void Initialize(Sprite sprite, UnityAction<MapNode> clickCallback, MapNode nodeData)
    {
        // Устанавливаем изображение
        Image.sprite = sprite;

        // Запоминаем делегат и данные о ноде
        _clickCallback = clickCallback;
        _nodeData = nodeData;

        // Добавляем обработчик клика на кнопку
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(OnButtonClick);
    }

    // Обработчик клика по кнопке
    private void OnButtonClick()
    {
        // Если делегат и данные есть — вызываем обработчик
        _clickCallback?.Invoke(_nodeData);
    }
}
