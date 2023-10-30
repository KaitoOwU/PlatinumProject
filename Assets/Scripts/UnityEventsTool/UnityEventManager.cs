using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UnityEditor.PackageManager;


public class UnityEventManager : MonoBehaviour
{
    private static UnityEventManager instance;
    public static UnityEventManager Instance => instance;

    [SerializeField] private UnityEventData _eventsData;
    [SerializeField] private AudioSource _audioSource;

    private void Awake()
    {
        instance = this;
        InitEvents(_eventsData);
    }
    void InitEvents(UnityEventData data)
    {
        //get all scripts in scene and add listener from scriptable object

        foreach (var script in data.DataBase)
        {
            var instances = FindObjectsOfType(Type.GetType($"{script.ScriptName}, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"));
            foreach (var instance in instances)
            {
                foreach (var eventValue in script.Events)
                {
                    switch (eventValue.EventAction.EventType)
                    {
                        case EventTypeEnum.DEBUG:
                            UnityAction d = () => DebugMessage(eventValue.EventAction.DebugMessage);
                            ((UnityEvent)(instance.GetType().GetField(eventValue.EventName).GetValue(instance))).AddListener(d);
                            break;
                    }

                }
            }
        }
    }
    private void OnDisable()
    {
        //((UnityEvent)(instance.GetType().GetField(eventValue.EventName).GetValue(instance))).RemoveListener(d);
    }
    void CallAllEvents()
    {
        
    }
 
    public void DebugMessage(string msg)
    {
        Debug.Log(msg);
    }
    public void PlaySFX(AudioClip audioClip)
    {
        _audioSource.PlayOneShot(audioClip);
    }
    public void ScreenShake(float intensity)
    {
        Debug.Log($"ScreenShake at intensity : {intensity}");
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

    //public void DebugMessage(string msg)
    //{
    //    UnityEventManager.Instance.DebugMessage(msg);
    //}
    //public void PlaySFX(AudioClip audioClip)
    //{
    //    UnityEventManager.Instance.PlaySFX(audioClip);
    //}
    //public void ScreenShake(float intensity)
    //{
    //    UnityEventManager.Instance.ScreenShake(intensity);
    //}
}
[Serializable]
public class UnityEventInfo
{
    public UnityEventInfo(string _eventName, EventAction _eventAction)
    {
        eventName = _eventName;
        eventAction = _eventAction;
    }
    public string EventName
    {
        get { return eventName; }
        set { eventName = value; }
    }
    public EventAction EventAction
    {
        get { return eventAction; }
        set { eventAction = value; }
    }
    [SerializeField]
    private string eventName;
    [SerializeField]
    private EventAction eventAction;
}
[Serializable]
public enum EventTypeEnum
{
    NONE,
    DEBUG,
    PLAY_SOUND,
    SCREENSHAKE,
}
[Serializable]
public class EventAction
{
    public EventAction() 
    {
        eventType = EventTypeEnum.NONE;
    }



    public EventTypeEnum EventType
    {
        get { return eventType; } set { eventType = value; }
    }

    public string DebugMessage
    {
        get { return debugMessage; }
        set { debugMessage = value; }
    }
    public AudioClip Clip
    {
        get { return clip; }
        set { clip = value; }
    }
    public float Intensity
    {
        get { return intensity; }
        set { intensity = value; }
    }

    [SerializeField]
    private EventTypeEnum eventType;
    [SerializeField]
    private string debugMessage;
    [SerializeField]
    private AudioClip clip;
    [SerializeField]
    private float intensity;
}
