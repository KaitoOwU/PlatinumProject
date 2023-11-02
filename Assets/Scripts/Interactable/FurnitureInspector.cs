using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Furniture;
using static UnityEditor.PlayerSettings;

[CustomEditor(typeof(Furniture))]
public class FurnitureInspector : Editor
{
    string[] _options = new string[] { "1", "2", "3" };
    int _chosenIndex;
    public override void OnInspectorGUI()
    {
        Furniture data = (Furniture)target;
        data.FurnitureType = (Furniture.EFurnitureType)EditorGUILayout.EnumPopup("Furniture type :", data.FurnitureType);
        if (data.FurnitureType == Furniture.EFurnitureType.MOVABLE)
        {
            EditorGUILayout.Space(10);

            _chosenIndex = EditorGUILayout.Popup("Number of Players to push :", _chosenIndex, _options);
            data.NeededPlayersCount = int.Parse(_options[_chosenIndex]);
        }
        EditorGUILayout.Space(10);

        data.Model = (GameObject)EditorGUILayout.ObjectField("Furniture 3D Model :",data.Model, typeof(GameObject), true);

        //DrawDefaultInspector();
    }

}


