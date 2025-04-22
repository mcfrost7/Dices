#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NewDiceConfig))]
public class NewDiceConfigEditor : Editor
{
    private bool isEnemyDice = false;

    public override void OnInspectorGUI()
    {
        // Отрисовка стандартных полей
        base.OnInspectorGUI();

        // Горизонтальная линия-разделитель
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // Галочка для определения вражеского кубика
        isEnemyDice = EditorGUILayout.Toggle("Is Enemy Dice", isEnemyDice);

        // Кнопки управления
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