using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerTeamUnitStats : MonoBehaviour
{
    [SerializeField] private GameObject image_level_panel_;

    [SerializeField] private TextMeshProUGUI text_hp;
    [SerializeField] private TextMeshProUGUI text_moral;
    [SerializeField] private TextMeshProUGUI text_level;


    public void DrawAllPanels(UnitStats stats)
    {
        DrawStatsPanel(stats);
    }

    private void DrawStatsPanel(UnitStats stats)
    {
        image_level_panel_.GetComponent<Image>().sprite = stats.Type.Sprite_level;
        text_hp.text = stats.CurrentHealth.ToString() + " / " + stats.Health.ToString();
        text_moral.text = stats.Moral.ToString();
        text_level.text = stats.Experience.ToString() + " / " + (stats.Type.Level*10).ToString();

    }

}