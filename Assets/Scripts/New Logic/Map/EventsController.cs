using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventsController : MonoBehaviour 
{
    private bool camping = false;

    [SerializeField] private UIController _controller;
    [SerializeField] private RouletteScreen _roulette;
    public bool Camping { get => camping; set => camping = value; }


    public void SetupBattle(CanvasMapGenerator.MapNode node)
    {

    }

    public void SetupResources()
    {

    }

    public void SetupRoulete()
    {
        _roulette.SetupRoulette();
        _controller.ShowRoulette();
    }

    public void SetupCamp()
    {

        Camping = true;
    }

    public void SetupBossBattle()
    {

    }
}
