using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
public class GameStateManager : MonoBehaviour
{
    [SerializeField] private int numberOfUnits;
    [SerializeField] private TileConfig tileConfigMain;
    [SerializeField] private List<Resource> resources;


    public int NumberOfUnits { get => numberOfUnits; set => numberOfUnits = value; }
    public TileConfig TileConfigMain { get => tileConfigMain; set => tileConfigMain = value; }

    public void StartNewGame()
    {
        CreateNewTeamForPlayer();
        LoadNewConfig();
        SetNewDifficulty();
        InitResources();
    }

    public void ContinueGame()
    {

    }

    private void CreateNewTeamForPlayer()
    {
        gameObject.GetComponent<GameManager>().Player.Units.Clear();
        gameObject.GetComponent<GameManager>().TeamManager1.GetComponent<TeamManager>().CreateUnit(NumberOfUnits);
    }

    private void LoadNewConfig()
    {
        gameObject.GetComponent<GameManager>().MapManager1.GetComponent<TileManager>().Initialize(TileConfigMain);
    }
    private void SetNewDifficulty()
    {
        GameManager.Instance.Player.Difficulty = 1;
    }

    private void InitResources()
    {
        foreach (var resource in resources)
        {
            Resource newResource = ScriptableObject.CreateInstance<Resource>();
            newResource.Initialize(resource.Name, resource.Amount, resource.Icon, resource.ResourcesType);
            GameManager.Instance.Player.Resources.Add(newResource);
        }
        gameObject.GetComponent<GameManager>().MenuManager1.GetComponent<UIControllerResources>().SpawnResources();
    }
}
