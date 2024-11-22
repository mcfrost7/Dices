using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private int numberOfUnits;
    [SerializeField] private TileConfig tileConfigMain;

    public int NumberOfUnits { get => numberOfUnits; set => numberOfUnits = value; }
    public TileConfig TileConfigMain { get => tileConfigMain; set => tileConfigMain = value; }

    public void StartNewGame()
    {
        CreateNewTeamForPlayer();
        LoadNewConfig();
        SetNewDifficulty();
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
}
