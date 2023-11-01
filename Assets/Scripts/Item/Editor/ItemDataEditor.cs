using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PickableData))]
public class ItemDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PickableData data = (PickableData)target;
        
        GUILayout.Label($"({data.ID}) {data.Name}", new GUIStyle(GUI.skin.label) {fontSize = 17, fontStyle = FontStyle.Bold});
        GUI.enabled = false;
        DrawDefaultInspector();
        GUI.enabled = true;  
        
        if (GUILayout.Button("Modifier"))
        {
            ItemManagerEditor.Init();
            ItemCreatorEditor.InitForModification(data);
        }
    }
}
