using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouletteScreen : MonoBehaviour
{
    [SerializeField] private RouletteView _view;
    [SerializeField] private Button _spinButtton;
    Roulette roulette;
    public void SetupRoulette(List<RouletteConfig> _configs)
    {
        _spinButtton.interactable = true;   
        RouletteConfig _currentConfig = new RouletteConfig();
        _currentConfig = _configs[UnityEngine.Random.Range(0, _configs.Count)]; 
        roulette = new Roulette(_view, _currentConfig);
        roulette.ClearRoulette();
        roulette.GenerateRoulette();
        AddButtonListener();

    }

    private void AddButtonListener()
    {
        _spinButtton.onClick.AddListener(SpinRoulette);
    }

    private void SpinRoulette()
    {
        StartCoroutine(SpinRouletteCoroutine());
    }

    private IEnumerator SpinRouletteCoroutine()
    {
        _spinButtton.interactable = false;
        yield return StartCoroutine(roulette.SpinRoulette());
        GameWindowController.Instance.CallPanel(1);
        GameWindowController.Instance.SetupRouletteInfo(roulette.GetConfigByAngle());
    }

}
