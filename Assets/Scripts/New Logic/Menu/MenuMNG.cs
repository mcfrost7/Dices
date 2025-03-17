using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMNG : MonoBehaviour
{
    [SerializeField] private GameObject _menuWindow;
    [SerializeField] private GameObject _InfoPanel;
    [SerializeField] private GameObject _freeze;
    [SerializeField] private GameObject _task;

    private GameObject _currentActiveWindow;
    private bool _taskVisibility = false;

    public void ShowWindow(GameObject _window)
    {
        _window.SetActive(true);
        _freeze.SetActive(true);
        _currentActiveWindow = _window;
    }

    public void HideWindow()
    {
        _freeze.SetActive(false);
        _currentActiveWindow.SetActive(false);
    }

    public void ShowInfo()
    {
        _currentActiveWindow = _InfoPanel;
        _freeze.SetActive(true);
    }

    public void CallTask()
    {
        if (_taskVisibility == false)
        {
            _task.transform.DOLocalMoveX(_task.transform.localPosition.x - 500, 0.8f).SetEase(Ease.InOutExpo);
        } else
        {
            _task.transform.DOLocalMoveX(_task.transform.localPosition.x + 500, 0.8f).SetEase(Ease.InOutExpo);
        }
        _taskVisibility = !_taskVisibility;
    }
    
    public void ChangeVisibilityOfDownPanel(GameObject _panel)
    {
        _panel.SetActive(!_panel.activeSelf);
    }

    public void TurnOnUI(GameObject _gameObject)
    {
        _gameObject.SetActive(true);
    }
    public void TurnOffUI(GameObject _gameObject)
    {
        _gameObject.SetActive(false);
    }
}
