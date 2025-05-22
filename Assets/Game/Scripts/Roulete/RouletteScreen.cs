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
    private bool _isRewarded = false;
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
        _isRewarded = false;
        _spinButtton.interactable = false;
        StartCoroutine(SpinAndHandleResult());
    }

    private IEnumerator SpinAndHandleResult()
    {
        yield return StartCoroutine(roulette.SpinRoulette());
        var config = roulette.GetConfigByAngle();

        if (_isRewarded == false)
        {
            GameWindowController.Instance.SetupRouletteInfo(config);
            GlobalWindowController.Instance.HideAllCanvases();
            _isRewarded = true;
        }
        GameWindowController.Instance.CallPanel(1);
    }


}
