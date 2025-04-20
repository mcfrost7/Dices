using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewDiceConfig", menuName = "Dice/Dice Config")]
public class NewDiceConfig : ScriptableObject
{
    public List<DiceSide> sides = new List<DiceSide>();
    public Sprite _unitSprite;

    // Метод для добавления новой стороны с автоматическим индексом
    public void AddNewSide(DiceSide newSide)
    {
        newSide.sideIndex = sides.Count;
        sides.Add(newSide);
    }

    // Метод для обновления индексов всех сторон (на случай удаления/перестановки)
    public void RefreshSideIndices()
    {
        for (int i = 0; i < sides.Count; i++)
        {
            sides[i].sideIndex = i;
        }
    }
}