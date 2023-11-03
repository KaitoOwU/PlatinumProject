using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UnityEditor.PackageManager;
using Unity.VisualScripting;
using DG.Tweening;
using static System.Collections.Specialized.BitVector32;

public class UnityEventManager : MonoBehaviour
{
    private static UnityEventManager instance;
    public static UnityEventManager Instance => instance;

    [Header("---References---")]
    [SerializeField] private UnityEventData _eventsData;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private GameObject _camera;

    //private List<UnityAction> _actions = new();

    // from script name : get 
    List<SAction> _actions = new();
    struct SAction
    {
        public SAction(UnityEvent _eventRef, UnityAction _action)
        {
            eventRef = _eventRef;
            action = _action;
        }
        public UnityEvent EventRef => eventRef;
        public UnityAction Action => action;
        UnityEvent eventRef;
        UnityAction action;
    }

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
            foreach (var instance in instances) // foreach class of type in scene
            {
                foreach (var eventValue in script.Events) // foreach event in class
                {
                    foreach(var eventAction in eventValue.EventActions)
                    {
                        UnityAction d = null;
                        switch (eventAction.EventType)
                        {
                            case EventTypeEnum.NONE:
                                break;
                            case EventTypeEnum.DEBUG:
                                d = () => DebugMessage(eventAction.DebugMessage);
                                break;
                            case EventTypeEnum.SCREENSHAKE:
                                d = () => ScreenShake(eventAction.Intensity);
                                break;
                            case EventTypeEnum.PLAY_SOUND:
                                d = () => PlaySFX(eventAction.Clip);
                                break;
                        }
                        if(d !=null)
                            ((UnityEvent)(instance.GetType().GetField(eventValue.EventName).GetValue(instance))).AddListener(d);
                        Debug.Log(eventValue.EventName);
                        Debug.Log((UnityEvent)(instance.GetType().GetField(eventValue.EventName).GetValue(instance)));
                        var t = (UnityEvent)(instance.GetType().GetField(eventValue.EventName).GetValue(instance));
                        _actions.Add(new SAction(t,d));
                    }

                }
            }
        }
    }
    private void OnDisable()
    {
        foreach(var a in _actions)
        {
            a.EventRef.RemoveListener(a.Action);
        }
        //((UnityEvent)(instance.GetType().GetField(eventValue.EventName).GetValue(instance))).RemoveListener(d);
    }
    void CallAllEvents()
    {
        
    }
 
    public void DebugMessage(string msg)
    {
        Debug.Log($"<color=cyan>{msg}</color>");
    }
    public void PlaySFX(AudioClip audioClip)
    {
        _audioSource.PlayOneShot(audioClip, 1);
    }
    public void ScreenShake(float intensity)
    {
        _camera.transform.DOShakePosition(intensity);
        Debug.Log($"ScreenShake at intensity : {intensity}");
    }
}

[Serializable]
public class ScriptEventInfo
{
    public ScriptEventInfo(string _scriptName, List<UnityEventInfo> _events)
    {
        scriptName = _scriptName;
        events = _events;
    }
    public string ScriptName
    {
        get { return scriptName; }
        set { scriptName = value; }
    }
    public List<UnityEventInfo> Events
    {
        get { return events; }
        set { events = value; }
    }

    [SerializeField]
    private string scriptName;
    [SerializeField]
    private List<UnityEventInfo> events;

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
    public UnityEventInfo(string _eventName, List<EventAction> _eventActions)
    {
        eventName = _eventName;
        eventActions = _eventActions;
    }
    public string EventName
    {
        get { return eventName; }
        set { eventName = value; }
    }
    public List<EventAction> EventActions
    {
        get { return eventActions; }
        set { eventActions = value; }
    }
    [SerializeField]
    private string eventName;
    [SerializeField]
    private List<EventAction> eventActions;
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
