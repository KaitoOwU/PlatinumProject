using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static Furniture;
using static UnityEditor.PlayerSettings;

//[CustomEditor(typeof(Furniture))]
public class FurnitureInspector : MonoBehaviour
{
    //string[] _options = new string[] { "1", "2", "3" };
    //int _chosenIndex;
    //[SerializeField]
    //SerializedProperty FurnitureType;
    //[SerializeField]
    //SerializedProperty NeededPlayersCount;
    //[SerializeField]
    //SerializedProperty Model;

    //public override void OnInspectorGUI()
    //{
    //    serializedObject.Update();
    //    Furniture obj = (Furniture)target;
    //    Furniture data = obj;
    //    //SerializedObject _serializedObject = new UnityEditor.SerializedObject(obj);

    //    FurnitureType = serializedObject.FindProperty("FurnitureType");
    //    NeededPlayersCount = serializedObject.FindProperty("NeededPlayersCount");
    //    Model = serializedObject.FindProperty("Model");

    //    if (FurnitureType == null)
    //        FurnitureType.enumValueIndex = 0;
    //    FurnitureType.enumValueIndex = (int)(Furniture.EFurnitureType)EditorGUILayout.EnumPopup("Furniture type :", data.FurnitureType);
    //    if (FurnitureType.enumValueIndex == (int)Furniture.EFurnitureType.MOVABLE)
    //    {
    //        EditorGUILayout.Space(10);

    //        //position.x = EditorGUIUtility.currentViewWidth - position.width - 20f;
    //        _chosenIndex = EditorGUILayout.Popup("Number of Players to push :", _chosenIndex, _options);
    //        NeededPlayersCount.intValue = int.Parse(_options[_chosenIndex]);
    //    }
    //    EditorGUILayout.Space(10);

    //    Model.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("Furniture 3D Model :",data.Model, typeof(GameObject), true);

    //    EditorUtility.SetDirty(target);
    //    //_serializedObject.FindProperty("FurnitureType").enumValueIndex = (int)data.FurnitureType;
    //    //_serializedObject.FindProperty("NeededPlayersCount").intValue = data.NeededPlayersCount;
    //    //_serializedObject.FindProperty("Model").objectReferenceValue = data.Model;

    //    serializedObject    .ApplyModifiedProperties();
    //}

}


