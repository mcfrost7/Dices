using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleActionManager : MonoBehaviour
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
    public static BattleActionManager Instance { get; private set; }

    public void CalculateAvailableActions() { }
}
