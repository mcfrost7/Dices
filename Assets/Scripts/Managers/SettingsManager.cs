using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] Canvas canvas_settings;
    [SerializeField] Button button_back;
    private GameObject manager_to_enable = null;

    public GameObject Manager_to_enable { get => manager_to_enable; set => manager_to_enable = value; }

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        canvas_settings.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        manager_to_enable = null;
        canvas_settings.gameObject.SetActive(false);
    }

    public void Button_Back()
    {

        // Включаем менеджер, который вы передали в Manager_to_enable
        if (Manager_to_enable != null)
        {
            Manager_to_enable.SetActive(true);
        }
        gameObject.SetActive(false);

    }

}
