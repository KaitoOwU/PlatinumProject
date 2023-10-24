using Codice.CM.WorkspaceServer.Tree.GameUI.HeadTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;
using UnityEngine.WSA;
using static UnityEngine.GraphicsBuffer;

public class EventEditor : EditorWindow
{

    static EventEditor _eventWindow;

    private DefaultAsset _targetFolder = null;
    List<ScriptEventInfo> _scriptEventInfo;
    class ScriptEventInfo{
        public ScriptEventInfo(MonoBehaviour _monoBehaviour, UnityEventInfo[] _events)
        {
            monoBehaviour = _monoBehaviour;
            events = _events;
        }
        public string ScriptName => monoBehaviour.name;
        public MonoBehaviour MonoBehaviour {
            get { return monoBehaviour; }
            set { monoBehaviour = value; }
        }
        public UnityEventInfo[] Events
        {
            get { return events; }
            set { events = value; }
        }

        MonoBehaviour monoBehaviour;
        UnityEventInfo[] events;
    }
    struct UnityEventInfo
    {
        public UnityEventInfo(string _eventName, UnityEvent _unityEvent)
        {
            eventName = _eventName;
            unityEvent = _unityEvent;
        }
        public string EventName
        {
            get { return eventName; }
            set { eventName = value; }
        }
        public UnityEvent UnityEvent
        {
            get { return unityEvent; }
            set { unityEvent = value; }
        }

        string eventName;
        UnityEvent unityEvent;
    }


    [MenuItem("Toolbox/Events")]
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

        if (_targetFolder != null)
            EditorGUILayout.HelpBox("Valid folder! Name: " + _targetFolder.name, MessageType.Info, true);
        else        
            EditorGUILayout.HelpBox("Not valid!", MessageType.Warning, true);


        
    }

    void UpdateDatabase(string folder)
    {
        var monoBehaviours = FindAllAssetsOfType<GameManager>(folder);
        foreach (var monoBehaviour in monoBehaviours)
        {
            FieldInfo[] unityEventsFields = monoBehaviour.GetType().GetFields().Where(f => f.FieldType == typeof(UnityEvent)).ToArray();

            UnityEventInfo[] newUnityEventsInfo =  new UnityEventInfo[unityEventsFields.Length];

            for(int i = 0; i < unityEventsFields.Length; i++)
            {
                newUnityEventsInfo[i].EventName = unityEventsFields[i].Name;
                newUnityEventsInfo[i].UnityEvent = (UnityEvent)unityEventsFields[i].GetValue(null);
            }

            ScriptEventInfo newInfo = new ScriptEventInfo(monoBehaviour, newUnityEventsInfo);

            ScriptEventInfo oldInfo = _scriptEventInfo.FirstOrDefault(f => f.ScriptName == newInfo.ScriptName);


            if(oldInfo == null)
                _scriptEventInfo.Add(newInfo);
            else
            {
                foreach(UnityEventInfo newEvent in newInfo.Events)
                {
                    // if old info of monobehaviour exist, only add events that donc exist already
                    //if(newEvent.EventName = oldInfo
                }
            }
        }
    }

    public static List<T> FindAllAssetsOfType<T>(string folder = "Assets") => AssetDatabase.FindAssets(folder).OfType<T>().ToList();

}
