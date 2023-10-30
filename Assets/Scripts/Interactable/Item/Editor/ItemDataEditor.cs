using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ItemData data = (ItemData)target;
        
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
