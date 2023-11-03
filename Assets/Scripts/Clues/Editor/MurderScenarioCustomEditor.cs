using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MurderScenario))]
public class MurderScenarioCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        serializedObject.Update();
        
        DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
        GUI.enabled = true;
        
        EditorUtility.SetDirty(target);
    }
}
