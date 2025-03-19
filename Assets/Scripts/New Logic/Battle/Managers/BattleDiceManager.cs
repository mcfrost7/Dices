using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDiceManager : MonoBehaviour
{
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    public static BattleDiceManager Instance { get; private set; }

    public void RollAllDice() { }
    public void ExecuteRerolls() { }
}