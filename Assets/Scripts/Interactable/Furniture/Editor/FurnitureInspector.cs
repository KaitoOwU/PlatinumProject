using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static Codice.Client.BaseCommands.Import.Commit;
using static Furniture;
using static UnityEditor.PlayerSettings;

[CustomEditor(typeof(Furniture))]
public class FurnitureInspector : Editor
{
    [SerializeField]
    SerializedProperty FurnitureType;
    [SerializeField]
    SerializedProperty NeededPlayersCount;
    [SerializeField]
    SerializedProperty Model;
    public Font font;

    public override void OnInspectorGUI()
    {
        GUI.skin.font = font;
        serializedObject.Update();
        Furniture data = (Furniture)target;

        FurnitureType = serializedObject.FindProperty("_furnitureType");
        NeededPlayersCount = serializedObject.FindProperty("_playersNeededNumber");
        Model = serializedObject.FindProperty("_3Dmodel");

        if (FurnitureType == null)
            FurnitureType.enumValueIndex = 0;

        if (FurnitureType.enumValueIndex == (int)EFurnitureType.MOVABLE)
            GUI.backgroundColor = new Color(0.8f, 1f, 0.7f);
        else
            GUI.backgroundColor = new Color(0.6f, 0.7f, 1f);
        FurnitureType.enumValueIndex = (int)(EFurnitureType)EditorGUILayout.EnumPopup("Furniture type :", data.FurnitureType);


        if (FurnitureType.enumValueIndex == (int)EFurnitureType.MOVABLE)
        {
            EditorGUILayout.Space(10);
            GUI.backgroundColor = new Color(1f, 1f, 1f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Number of Players to push :");
            NeededPlayersCount.intValue = GUILayout.Toolbar(NeededPlayersCount.intValue - 1, new string[] {"1", "2", "3"}) + 1;
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Space(10);

        GUI.backgroundColor = new Color(1f, 1f, 1f);
        Model.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("Furniture 3D Model :", data.Model, typeof(GameObject), true);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }
    }
}


