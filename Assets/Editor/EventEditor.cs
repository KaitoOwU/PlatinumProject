using Codice.CM.WorkspaceServer.Tree.GameUI.HeadTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;
using UnityEngine.WSA;
using static UnityEngine.GraphicsBuffer;

public class EventEditor : EditorWindow
{

    static EventEditor _eventWindow;

    static private DefaultAsset _targetFolder = null;
    static private DefaultAsset _dataFolder = null;
    List<ScriptEventInfo> _database;
    UnityEventData _databaseScriptableObject;
    SerializedObject _serializedDatabase;

    int count = 0;


    [MenuItem("Platinum/Events")]
    static void InitWindow()
    {

        _eventWindow = GetWindow<EventEditor>();
        //_window.maxSize = new Vector2(200,100);
        _eventWindow.titleContent = new GUIContent("Events");

        _eventWindow.Show();
    }

    private void OnGUI()
    {
        _targetFolder = (DefaultAsset)EditorGUILayout.ObjectField("Select Script Folder", _targetFolder, typeof(DefaultAsset), false);
        _dataFolder = (DefaultAsset)EditorGUILayout.ObjectField("Select folder to store Data", _dataFolder, typeof(DefaultAsset), false);



        //if (_serializedDatabase != null)
        //{
        //    if (_serializedDatabase.FindProperty("serializedProperty") == null)
        //        Debug.Log(_serializedDatabase.GetIterator().name);
        //    else
        //        EditorGUILayout.PropertyField(_serializedDatabase.FindProperty("serializedProperty"));
        //}
        //else
        //    Debug.Log("Not yet");
        if (_targetFolder != null)
            EditorGUILayout.HelpBox("Valid folder! Name: " + _targetFolder.name, MessageType.Info, true);
        else
            EditorGUILayout.HelpBox("Not valid!", MessageType.Warning, true);
        if (GUILayout.Button("Update Database"))
        {
            _database = UpdateDatabase(AssetDatabase.GetAssetPath(_targetFolder));
            //Debug.Log("_database.Count " + _database.Count);
            count++;
            UnityEventData _databaseScriptableObject = CreateInstance<UnityEventData>();
            _serializedDatabase = new UnityEditor.SerializedObject(_databaseScriptableObject);
            AssetDatabase.CreateAsset(_databaseScriptableObject, AssetDatabase.GetAssetPath(_dataFolder) + "/EventData_" + count + ".asset");
            var path = AssetDatabase.GetAssetPath(_databaseScriptableObject);
            _databaseScriptableObject.DataBase = _database;
            AssetDatabase.SaveAssets();
        }

    }
    Type GetTypeByName(string name)
    {
        Type t;
        foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (ass.FullName.StartsWith("System."))
                continue;
            t = ass.GetType(name);
            if (t != null)
                return t;
        }
        //Debug.Log("null type");
        return null;
    }

    List<ScriptEventInfo> UpdateDatabase(string folder)
    {
        var scripts = FindAllScripts(folder);
        List<ScriptEventInfo> newDatabase = new();
        foreach (var script in scripts)
        {
            var type = GetTypeByName(script);
            var fields = type.GetFields();
            var where = fields.Where(f => f.FieldType == typeof(UnityEvent));
            FieldInfo[] unityEventsFields = where.ToArray();
            //Debug.Log(script + " " + unityEventsFields.Length + " " + fields.Length);
            //foreach (FieldInfo field in fields)
            //    Debug.Log("Fields : "+ script+ "   :   " + field.Name);
            if (unityEventsFields.Length == 0)
                continue;
            UnityEventInfo[] newUnityEventsInfo = new UnityEventInfo[unityEventsFields.Length];


            for (int i = 0; i < unityEventsFields.Length; i++)
            {
                newUnityEventsInfo[i] = new(unityEventsFields[i].Name, new UnityEvent());
            }

            ScriptEventInfo newScriptInfo = new ScriptEventInfo(script, newUnityEventsInfo);

            ScriptEventInfo oldScriptInfo = _database.FirstOrDefault(f => f.ScriptName == newScriptInfo.ScriptName);


            if (oldScriptInfo != null)
            {
                //verify if old events still exist => dont replace // only recreate if didnt exist because it creates an empty event
                for (int i = 0; i < newScriptInfo.Events.Length; i++)
                {
                    UnityEventInfo oldEvent = oldScriptInfo.Events.FirstOrDefault(f => f.EventName == newScriptInfo.Events[i].EventName);
                    if (oldEvent != null)
                    {
                        newScriptInfo.Events[i] = oldEvent;
                    }
                }
            }
            if (newScriptInfo.Events.Length>0) 
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

}
