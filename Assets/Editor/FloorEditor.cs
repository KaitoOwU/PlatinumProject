using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System;
using static SCRoomsLists;


public class FloorEditor : EditorWindow
{
    private string FolderPath = "Assets/Resources/ScriptableObject/";
    private string _extention = ".asset";
    private Vector2 _windowSize;
    private SCRoomsLists _floors;
    private Floor _editedRooms;
    private string _editedRoomsName;
    Vector2 _scrollPos;

    #region SetUp
    [MenuItem("Platinum/FloorCreator")]
    private static void Init()
    {
        FloorEditor window = GetWindowWithRect<FloorEditor>(new Rect(0, 0, 500, 300), false);
        window.Show();
    }
    private void OnGUI()
    {
        LoadPaths();
        OnGUIUpdate();
    }

    private void LoadPaths()
    {
        SCRoomsLists paths = Resources.Load<SCRoomsLists>("ScriptableObject/Rooms");
        _floors = paths;
    }
    private void OnGUIUpdate()
    {
        EditorGUILayout.BeginVertical();
        DisplayFloorList();
        DisplayRooms();
        EditorGUILayout.EndVertical();
    }
    #endregion
    #region Display
    private void DisplayFloorList()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Room type List:",  new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < _floors.Floors.Count; i++)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Label((i + 1) + " Doors Rooms :",new GUIStyle(GUI.skin.label) { fontSize = 12, alignment = TextAnchor.MiddleCenter });
            DisplayListElement(_floors.Floors[i], i);
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
    private void DisplayListElement(Floor floor, int index)
    {
        if (GUILayout.Button("Edit", new GUIStyle(GUI.skin.button) { fontSize = 10, fontStyle = FontStyle.Bold }))
        {
            _editedRoomsName = (index+ 1) + " Doors Rooms :";
            _editedRooms = floor;
        }
    }
    private void DisplayRooms()
    {

        if (_editedRooms == null)
        {
            _editedRooms = _floors.Floors[0];
            _editedRoomsName ="1 Doors Rooms :";
        }
        EditorGUILayout.BeginVertical();
        
        GUILayout.Label(_editedRoomsName, new GUIStyle(GUI.skin.label) { fontSize = 15, fontStyle = FontStyle.Bold, alignment= TextAnchor.MiddleCenter });
        _scrollPos =EditorGUILayout.BeginScrollView(_scrollPos,GUILayout.Height(180));   
        if (_editedRooms.Rooms.Count > 0)
        {
            for (int i = 0; i < _editedRooms.Rooms.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Room " + i );
                
                _editedRooms.Rooms[i]= EditorGUILayout.ObjectField( _editedRooms.Rooms[i], typeof(GameObject),false) as GameObject;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("X", new GUIStyle(GUI.skin.button) { fixedHeight = 18, fontSize = 10, fontStyle = FontStyle.Bold,alignment=TextAnchor.MiddleCenter }))
                {
                    _editedRooms.Rooms.RemoveAt(i);
                }
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("Add Room", new GUIStyle(GUI.skin.button) { fixedHeight = 16, fontSize = 11, fontStyle = FontStyle.Bold }))
        {
            _editedRooms.Rooms.Add(new GameObject());
        }
        GUI.backgroundColor = new Color(0, .53f, .22f);
        if (GUILayout.Button("Save", new GUIStyle(GUI.skin.button) { fixedHeight = 16, fontSize = 11, fontStyle = FontStyle.Bold, }))
        {
            SaveRooms();
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndVertical();
    }
    #endregion
    private void SaveRooms()
    {
        string path = $"{FolderPath}Rooms{_extention}";
        EditorUtility.SetDirty(_floors);
        if (!File.Exists(path))
        {
            Debug.Log("ça marche?");
            AssetDatabase.CreateAsset(_floors, path);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}


