using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileUIController : MonoBehaviour
{
    public static TileUIController Instance { get; private set; }

    [SerializeField] private Button _healBbutton;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeUIOnTileClick()
    {
        ChangeHealButtonAvailabelness();
    }


    public void ChangeHealButtonAvailabelness()
    {
        if (GameDataMNG.Instance.CurrentTile.tileType == TileType.CampTile)
        {
            _healBbutton.gameObject.SetActive(true);
        }
        else _healBbutton.gameObject.SetActive(false);
    }

}
