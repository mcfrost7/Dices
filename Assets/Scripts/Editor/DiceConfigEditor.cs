#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NewDiceConfig))]
public class NewDiceConfigEditor : Editor
{
    private bool isEnemyDice = false;

    public override void OnInspectorGUI()
    {
        // ��������� ����������� �����
        base.OnInspectorGUI();

        // �������������� �����-�����������
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // ������� ��� ����������� ���������� ������
        isEnemyDice = EditorGUILayout.Toggle("Is Enemy Dice", isEnemyDice);

        // ������ ����������
        EditorGUILayout.Space();
        if (GUILayout.Button("Add New Side"))
        {
            DiceSide newSide = new DiceSide();
            ((NewDiceConfig)target).AddNewSide(newSide);
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Refresh All Indices"))
        {
            ((NewDiceConfig)target).RefreshSideIndices();
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Load Unit Meta Data"))
        {
            ((NewDiceConfig)target).LoadImages(isEnemyDice);
            EditorUtility.SetDirty(target);
        }
    }
}
#endif