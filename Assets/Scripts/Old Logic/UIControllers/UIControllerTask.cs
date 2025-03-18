using DG.Tweening;
using TMPro;
using UnityEngine;

public class TaskButtonController : MonoBehaviour
{
    [Header("Текст задачи")]
    [TextArea(3, 10)]
    [SerializeField] private string message;
    [SerializeField] private string message_name;
    [SerializeField] private GameObject panel_main;
    [SerializeField] private TextMeshProUGUI text_info;
    [SerializeField] private TextMeshProUGUI text_name;
    private Vector3 start_position;

    private bool isPanelVisible = false; // Переменная для отслеживания состояния панели

    private void OnDisable()
    {
        if (isPanelVisible)
        {
            ButtonTaskClick();
        }
    }

    public void ButtonTaskClick()
    {
        text_info.text = message;
        text_name.text = message_name;
        if (isPanelVisible == false)
        {
            panel_main.transform.DOLocalMoveX(panel_main.transform.localPosition.x - 560, 0.8f).SetEase(Ease.InOutExpo); // Добавляем смещение к текущей позиции
        }
        else
        {
            panel_main.transform.DOLocalMoveX(panel_main.transform.localPosition.x + 560, 0.8f).SetEase(Ease.InOutExpo); // Возвращаем панель в исходное положение
        }

        isPanelVisible = !isPanelVisible; // Переключение состояния
    }
}
