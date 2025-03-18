using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI: MonoBehaviour
{
    public static BattleUI Instance { get; private set; }

    // UI panel methods
    public void ShowIntentionPanel() { }
    public void HideIntentionPanel() { }
    public bool AreIntentionsConfirmed() { return false; }

    public void ShowDiceRolling() { }
    public void ShowDiceResults() { }

    public void ShowRerollPanel() { }
    public void HideRerollPanel() { }
    public bool AreRerollsConfirmed() { return false; }

    public void ShowActionPanel() { }
    public void HideActionPanel() { }
    public bool AreActionsComplete() { return false; }

    public void ShowVictoryScreen() { }
    public void HideVictoryScreen() { }

    public void ShowDefeatScreen() { }
    public void HideDefeatScreen() { }
}
