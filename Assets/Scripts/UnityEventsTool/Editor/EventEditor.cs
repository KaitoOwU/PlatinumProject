using DG.DemiEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class EventEditor : EditorWindow
{
    static EventEditor _eventWindow;
    static private UnityEngine.Object _targetScriptFolder = null;
    static private UnityEngine.Object _saveDataFolder = null;
    static private UnityEngine.Object _currentDataFolder = null;
    private List<ScriptEventInfo> _database;
    private UnityEventData _databaseScriptableObject;
    public Font font;

    [MenuItem("Platinum/Events")]
    static void InitWindow()
    {

        _eventWindow = GetWindow<EventEditor>();
        _eventWindow.minSize = new Vector2(400, 410);
        _eventWindow.titleContent = new GUIContent("Events");
        _eventWindow.Show();

        _targetScriptFolder = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(EditorPrefs.GetString("TargetScriptFolder"));
        _saveDataFolder = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(EditorPrefs.GetString("SaveDataFolder"));
        _currentDataFolder = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(EditorPrefs.GetString("CurrentDataFolder"));
    }

    private void OnGUI()
    {
        GUI.skin.font = font;
        if (!font)
        {
            Debug.LogError("No font found, assign one in the inspector.");
            return;
        }
        EditorGUILayout.Space(20);
        _targetScriptFolder = EditorGUILayout.ObjectField("Script Folder", _targetScriptFolder, typeof(DefaultAsset), false, GUILayout.Height(40));
        _saveDataFolder = EditorGUILayout.ObjectField("Data Saves Folder", _saveDataFolder, typeof(DefaultAsset), false, GUILayout.Height(40));
        _currentDataFolder = EditorGUILayout.ObjectField("Current Data Folder", _currentDataFolder, typeof(DefaultAsset), false, GUILayout.Height(40));

        if (EditorGUI.EndChangeCheck())
        {
            if (CheckFolder(_targetScriptFolder))
                EditorPrefs.SetString("TargetScriptFolder", AssetDatabase.GetAssetPath(_targetScriptFolder));
            if (CheckFolder(_saveDataFolder))
                EditorPrefs.SetString("SaveDataFolder", AssetDatabase.GetAssetPath(_saveDataFolder));
            if (CheckFolder(_currentDataFolder))
                EditorPrefs.SetString("CurrentDataFolder", AssetDatabase.GetAssetPath(_currentDataFolder));
        }

        EditorGUILayout.Space(20);
        GUIStyle buttonStyle = GUI.skin.button;
        buttonStyle.fontSize = 20;
        GUI.backgroundColor = new Color(0.1f, 0.5f, 0.8f);
        if (GUILayout.Button("Update Database", buttonStyle, GUILayout.Height(50)))
        {
            if (_targetScriptFolder != null && _saveDataFolder != null && _currentDataFolder != null)
            {
                _database = UpdateDatabase(AssetDatabase.GetAssetPath(_targetScriptFolder));

                _databaseScriptableObject = CreateInstance<UnityEventData>();
                MoveOldData();

                int assetCount = (new DirectoryInfo(EditorPrefs.GetString("SaveDataFolder"))).GetFiles().Length / 2;
                string newAssetName = AssetDatabase.GetAssetPath(_currentDataFolder) + "/EventData_" + assetCount + ".asset";
                AssetDatabase.CreateAsset(_databaseScriptableObject, newAssetName);
                EditorPrefs.SetString("CurrentDataDirectory", newAssetName);
                _databaseScriptableObject.DataBase = _database;
                AssetDatabase.SaveAssets();
            }
            else
            {
                EditorUtility.DisplayDialog("Warning",
                    "You have to assign all the folders first! \n\n-->Try to reopen the window if you already assigned them",
                    "My bad", "NO!");
            }
        }
        EditorGUILayout.Space(10);
        GUI.backgroundColor = new Color(0.1f, 0.7f, 0.6f);
        if (GUILayout.Button("Edit Database", buttonStyle, GUILayout.Height(50)))
        {
            if (EditorPrefs.GetString("CurrentDataDirectory") != "")
            {
                if(AssetDatabase.LoadAllAssetsAtPath(EditorPrefs.GetString("CurrentDataDirectory")).Length == 0)
                {
                    EditorUtility.DisplayDialog("Warning",
                    "No database has been found! \n\n-->Try to Update Database first",
                    "OK", "NO!");
                }
                else
                {
                    var currentDatabase = AssetDatabase.LoadAllAssetsAtPath(EditorPrefs.GetString("CurrentDataDirectory"))[0];
                    if (currentDatabase != null)
                    {
                        Selection.activeObject = currentDatabase;
                        EditorGUIUtility.PingObject(Selection.activeObject);
                    }
                }
            }
        }
    }
    List<ScriptEventInfo> UpdateDatabase(string folder)
    {
        var oldData = AssetDatabase.LoadAllAssetsAtPath(EditorPrefs.GetString("CurrentDataDirectory"));
        UnityEventData _oldDatabase = null;
        if (oldData.Length > 0)
        {
            //_oldDatabasePath = (new DirectoryInfo(EditorPrefs.GetString("CurrentDataFolder"))).GetFiles()[0];
            //_oldDatabasePath = EditorPrefs.GetString("CurrentDataFolder");
            _oldDatabase = (UnityEventData)(oldData[0]);
        }

        var scripts = FindAllScripts(folder);
        List<ScriptEventInfo> newDatabase = new();
        foreach (var script in scripts)
        {
            var type = Type.GetType($"{script}, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            if (type == null)
                continue; // Pass if script is editor 

            FieldInfo[] unityEventsFields = type.GetFields().Where(f => f.FieldType == typeof(UnityEvent)).ToArray();

            if (unityEventsFields.Length == 0)
                continue; // Pass if there is no events in script

            List<UnityEventInfo> newUnityEventsInfo = new List<UnityEventInfo>();


            for (int i = 0; i < unityEventsFields.Length; i++)
            {
                newUnityEventsInfo.Add(new(unityEventsFields[i].Name, new()));
            }

            ScriptEventInfo newScriptInfo = new ScriptEventInfo(script, newUnityEventsInfo);

            if(_oldDatabase != null)//if exist, compare with old database
            {
                ScriptEventInfo oldScriptInfo = _oldDatabase.DataBase.FirstOrDefault(f => f.ScriptName == newScriptInfo.ScriptName);

                if (oldScriptInfo != null)
                {
                    //verify if old events still exist => dont replace // only recreate if didnt exist because it creates an empty event
                    for (int i = 0; i < newScriptInfo.Events.Count; i++)
                    {
                        UnityEventInfo oldEvent = oldScriptInfo.Events.FirstOrDefault(f => f.EventName == newScriptInfo.Events[i].EventName);
                        if (oldEvent != null)
                            newScriptInfo.Events[i] = oldEvent;
                    }
                }
            }
            if (newScriptInfo.Events.Count > 0) 
                newDatabase.Add(newScriptInfo);
        }
        return (newDatabase);
    }

    public static List<string> FindAllScripts(string folder = "Assets") {
        var scriptsGUID = AssetDatabase.FindAssets("t:script", new[] { folder });
        List<string> classNames = new();
        foreach (var scriptGUID in scriptsGUID)
        {
            classNames.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(scriptGUID), typeof(MonoScript)).name);
        }
        return classNames;
    }

    bool CheckFolder(UnityEngine.Object folder)
    {
        if (folder != null)
        {
            EditorGUILayout.HelpBox("Valid folder! Name: " + folder.name, MessageType.Info, true);
            return true;
        }
        else
        {
            EditorGUILayout.HelpBox("Not valid!", MessageType.Warning, true);
            return false;
        }
    }

    void MoveOldData()
    {
        if (EditorPrefs.GetString("CurrentDataDirectory") == "")
            return;
        var oldData = AssetDatabase.LoadAllAssetsAtPath(EditorPrefs.GetString("CurrentDataDirectory"));
        if (oldData.Length == 0)
            return;
        string oldPath = EditorPrefs.GetString("CurrentDataDirectory");
        string newPath = EditorPrefs.GetString("SaveDataFolder");

        var assetName = AssetDatabase.LoadMainAssetAtPath(oldPath).name;


        var moveResult = AssetDatabase.ValidateMoveAsset(oldPath, newPath +"/"+ assetName + ".asset");

        if (moveResult == "")
            AssetDatabase.MoveAsset(oldPath, newPath + "/" + assetName + ".asset");
        else
            Debug.LogError($"Couldn't move {oldPath} because {moveResult}");

        //AssetDatabase.MoveAsset(oldPath, newPath + assetName +".asset");

        AssetDatabase.Refresh();
    }
}
