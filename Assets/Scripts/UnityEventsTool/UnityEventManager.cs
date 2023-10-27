using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventManager : MonoBehaviour
{
    private static UnityEventManager instance;
    public static UnityEventManager Instance => instance;

    [SerializeField] private UnityEventData eventsData;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //get all scripts in scene and add listener from scriptable object
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
[Serializable]
public class ScriptEventInfo
{
    public ScriptEventInfo(string _scriptName, UnityEventInfo[] _events)
    {
        scriptName = _scriptName;
        events = _events;
    }
    public string ScriptName
    {
        get { return scriptName; }
        set { scriptName = value; }
    }
    public UnityEventInfo[] Events
    {
        get { return events; }
        set { events = value; }
    }

    [SerializeField]
    private string scriptName;
    [SerializeField]
    private UnityEventInfo[] events;
}
[Serializable]
public class UnityEventInfo
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
    [SerializeField]
    private string eventName;
    [SerializeField]
    private UnityEvent unityEvent;
}
