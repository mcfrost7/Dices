using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIControllerNotification : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text_information;
    [SerializeField] GameObject button_confirm;
    [SerializeField] GameObject panel_main;

    private void OnEnable()
    {
        panel_main.SetActive(false);
    }

    private void SetUpText()
    {
        if (GameManager.Instance.Status == BattleStatus.Win)
            text_information.text = "Статус: ксеносы уничтожены.\n"
                + "Приказ: продолжайте продвижение.\n"
                + "Конец передачи.";
        else
        {
            text_information.text = "Статус: Поражение.\n"
               + "Приказ: ... \n"
               + "Конец передачи.";
        }
    }

    public void LoadInfoMenu()
    {
        SetUpText();
        panel_main.SetActive(true);
        gameObject.GetComponent<UIControllerGlobalMenu>().Image_freeze.SetActive(true);
    }

    public void ConfirmButtonClick()
    {
        panel_main.SetActive(false);
        gameObject.GetComponent<UIControllerGlobalMenu>().Image_freeze.SetActive(false);
        GameManager.Instance.EnableManager(GameManager.Instance.BattleManager1, false );
    }

}
