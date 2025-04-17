#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NewDiceConfig))]
public class NewDiceConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        NewDiceConfig config = (NewDiceConfig)target;

        if (GUILayout.Button("Add New Side"))
        {
            DiceSide newSide = new DiceSide();
            config.AddNewSide(newSide);
            EditorUtility.SetDirty(config);
        }

        if (GUILayout.Button("Refresh All Indices"))
        {
            config.RefreshSideIndices();
            EditorUtility.SetDirty(config);
        }
    }
}
#endif