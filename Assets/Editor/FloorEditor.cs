using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System;
using static SCRoomsLists;


public class PathCreator : EditorWindow
{
    private string FolderPath = "Assets/Resources/ScriptableObject/";
    private string _extention = ".asset";
    private Vector2 _windowSize;
    private SCRoomsLists _floors;
    private Floor _editedRooms;
    Vector2 _scrollPos;

    #region SetUp
    [MenuItem("Platinium/FloorCreator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PathCreator), false, "Floor Creator");
    }
    private void OnGUI()
    {
        InitGUI();
        LoadPaths();
        OnGUIUpdate();
    }

    private void LoadPaths()
    {
        SCRoomsLists paths = Resources.Load<SCRoomsLists>("ScriptableObject/Rooms");
        _floors = paths;
    }

    private void InitGUI()
    {
        _windowSize = new Vector2(position.width, position.height);

    }

    private void OnGUIUpdate()
    {
        EditorGUILayout.BeginHorizontal(SetOptionSize(_windowSize.x, _windowSize.x, _windowSize.y, _windowSize.y));
        DisplayFloorList();
        DisplayRooms();
        EditorGUILayout.EndHorizontal();
    }
    #endregion
    #region Display
    private void DisplayFloorList()
    {
        EditorGUILayout.BeginVertical(SetOptionSize(200, 200, _windowSize.y, _windowSize.y));
        GUILayout.Label("Floor List:", EditorStyles.boldLabel);
        if (GUILayout.Button("New Entry", SetOptionSize(200, 200, 20, 20)))
        {
            _editedRooms.Rooms = new List<GameObject>();
            _floors.Floors.Add(_editedRooms);
        }
        for (int i = 0; i < _floors.Floors.Count; i++)
        {
            DisplayListElement(_floors.Floors[i], i);
        }
        EditorGUILayout.EndVertical();
    }
    private void DisplayListElement(Floor floor, int index)
    {
        EditorGUILayout.BeginHorizontal(SetOptionSize(200, 200, 20, 20));
        if (GUILayout.Button("Edit", SetOptionSize(50, 50, 20, 20)))
            _editedRooms = floor;
        if (GUILayout.Button("-", SetOptionSize(50, 50, 20, 20)))
            _floors.Floors.Remove(floor);
        EditorGUILayout.EndHorizontal();
    }
    private void DisplayRooms()
    {
        EditorGUILayout.BeginVertical(SetOptionSize(200, 200, _windowSize.y, _windowSize.y));
        GUILayout.Label("Path:", EditorStyles.boldLabel);
        if (_editedRooms.Rooms.Count > 30)
        {
            _scrollPos =EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(500), GUILayout.Height(800));
        }
        if (_editedRooms.Rooms.Count > 0)
        {
            for (int i = 0; i < _editedRooms.Rooms.Count; i++)
            {
                EditorGUILayout.BeginHorizontal(SetOptionSize(100, 100, 20, 20));
                EditorGUILayout.LabelField("Room: ");
                
                _editedRooms.Rooms[i]= EditorGUILayout.ObjectField( _editedRooms.Rooms[i], typeof(GameObject),false, SetOptionSize(100, 100, 20, 20)) as GameObject;

                if (GUILayout.Button("X", SetOptionSize(20, 20, 20, 20)))
                {
                    _editedRooms.Rooms.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        if (_editedRooms.Rooms.Count > 30)
        {
            EditorGUILayout.EndScrollView();
        }
        if (GUILayout.Button("Add Room", SetOptionSize(340, 340, 20, 20)))
        {
            _editedRooms.Rooms.Add(new GameObject());
        }
        if (GUILayout.Button("Save", SetOptionSize(340, 340, 20, 20)))
        {
            SaveInputs();
        }

        EditorGUILayout.EndVertical();
    }
    #endregion
    private void SaveInputs()
    {
        string path = $"{FolderPath}Paths{_extention}";

        if (!File.Exists(path))
        {
            Debug.Log("Input doesn't exists");
            AssetDatabase.CreateAsset(_floors, path);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    #region GUIoptions
    public static GUILayoutOption[] SetOptionSize(int minWidth, int maxWidth, int minHeight, int maxHeight, int width = 0, int height = 0)
    {
        if (width == 0 || height == 0)
            return new GUILayoutOption[] {
            GUILayout.MinWidth(minWidth),
            GUILayout.MaxWidth(maxWidth),
            GUILayout.MinHeight(minHeight),
            GUILayout.MaxHeight(maxHeight)
        };
        return new GUILayoutOption[] {
            GUILayout.MinWidth(minWidth),
            GUILayout.MaxWidth(maxWidth),
            GUILayout.MinHeight(minHeight),
            GUILayout.MaxHeight(maxHeight),
            GUILayout.Width(width),
            GUILayout.Height(height)
        };
    }

    public static GUILayoutOption[] SetOptionSize(float minWidth, float maxWidth, float minHeight, float maxHeight, float width = 0, float height = 0)
    {
        if (width == 0 || height == 0)
            return new GUILayoutOption[] {
            GUILayout.MinWidth(minWidth),
            GUILayout.MaxWidth(maxWidth),
            GUILayout.MinHeight(minHeight),
            GUILayout.MaxHeight(maxHeight)
        };
        return new GUILayoutOption[] {
            GUILayout.MinWidth(minWidth),
            GUILayout.MaxWidth(maxWidth),
            GUILayout.MinHeight(minHeight),
            GUILayout.MaxHeight(maxHeight),
            GUILayout.Width(width),
            GUILayout.Height(height)
        };
    }
    #endregion //GUIoptions
}


