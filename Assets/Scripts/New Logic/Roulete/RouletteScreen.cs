using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouletteScreen : MonoBehaviour
{
    [SerializeField] private List<RouletteConfig> _configs;
    [SerializeField] private RouletteView _view;
    [SerializeField] private Button _spinButtton;
    Roulette roulette;
    public void SetupRoulette()
    {
       
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
        StartCoroutine(roulette.SpinRoulette());
    }
}
