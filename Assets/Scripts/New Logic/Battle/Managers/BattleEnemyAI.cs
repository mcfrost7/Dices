using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEnemyAI : MonoBehaviour
{
    public static BattleEnemyAI Instance { get; private set; }

    public void ExecuteActions() { }
    public bool AreActionsComplete() { return true; }
}
