using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewDiceConfig", menuName = "Dice/Dice Config")]
public class NewDiceConfig : ScriptableObject
{
    public List<DiceSide> sides = new List<DiceSide>();
    public Sprite _unitSprite;

    // ����� ��� ���������� ����� ������� � �������������� ��������
    public void AddNewSide(DiceSide newSide)
    {
        newSide.sideIndex = sides.Count;
        sides.Add(newSide);
    }

    // ����� ��� ���������� �������� ���� ������ (�� ������ ��������/������������)
    public void RefreshSideIndices()
    {
        for (int i = 0; i < sides.Count; i++)
        {
            sides[i].sideIndex = i;
        }
    }
}