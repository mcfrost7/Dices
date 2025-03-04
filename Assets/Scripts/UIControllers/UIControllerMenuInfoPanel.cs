using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIControllerMenuInfoPanel : MonoBehaviour
{
    [SerializeField] private GameObject main_panel;
    [SerializeField] private TextMeshProUGUI text_name;
    [SerializeField] private TextMeshProUGUI text_info;

    public GameObject Main_panel { get => main_panel; set => main_panel = value; }
    public TextMeshProUGUI Text_name { get => text_name; set => text_name = value; }
    public TextMeshProUGUI Text_info { get => text_info; set => text_info = value; }
    public void ChooseInfo(TileType tileType, int heal = 0, Resource resource = null)
    {
        switch (tileType)
        {
            case TileType.CampTile:
                SetUpTextCampfire(heal);
                break;
            case TileType.LootTile:
                SetUpTextLoot(resource);
                break;
        }
    }

    private void SetUpTextCampfire(int heal)
    {
        Text_name.text = "Форпост";
        Resource transmitter = null;
        foreach (var resources in GameManager.Instance.Player.Resources)
        {
            if (resources.ResourcesType == ResourcesType.SignalTransmitter)
            {
                transmitter = resources;
                break;
            }
        }
        if (transmitter.Amount > 0)
        { 
            Text_info.text = "Ваш отряд восстановил здоровье: " + heal + " единицы. \n" +
            "Потрачен 1 ВОКС-передатчик.";
            transmitter.SubtractAmount(1);
        }
        else
        {
            Text_info.text = "У вас нет ВОКС-передатчиков для восстановления.";
        }
    }

    private void SetUpTextLoot(Resource resource)
    {
        Text_name.text = "Ресурсы";
        Text_info.text = "Найден ресурс " + resource.name+ ": ед.";
    }

    public void OnButtonClick()
    {
        Main_panel.SetActive(false);
    }
}