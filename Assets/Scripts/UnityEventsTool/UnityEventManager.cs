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
using static UnityEngine.Rendering.DebugUI;

public class UnityEventManager : MonoBehaviour
{
    private static UnityEventManager instance;
    public static UnityEventManager Instance => instance;

    [Header("---References---")]
    [SerializeField] private UnityEventData _eventsData;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _audioSourcesStorage;

    //private List<UnityAction> _actions = new();

    private List<SSoundRef> _soundsPlaying = new();


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
            Debug.Log("script " + script.ScriptName);
            var instances = FindObjectsOfType(Type.GetType($"{script.ScriptName}, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"));
            foreach (var instance in instances) // foreach class of type in scene
            {
                Debug.Log("instance " + instance.name + " " + script.ScriptName);
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
                                if(eventAction.Intensity != 0)
                                    d = () => ScreenShake(eventAction.Intensity);
                                break;
                            case EventTypeEnum.PLAY_SOUND_ONCE:
                                if(eventAction.ClipAudio != null)
                                    d = () => PlaySFX(eventAction.ClipAudio);
                                break;
                            case EventTypeEnum.PLAY_RANDOM_SOUND_ONCE:
                                var nonNullClips = eventAction.ClipsAudio.Where(c => c != null).ToList();
                                if(nonNullClips.Count != 0)
                                    d = () => PlayRandomSFX(nonNullClips);
                                break;
                            case EventTypeEnum.START_PLAY_SOUND:
                                Debug.Log("START PlaySFX "+ eventAction.ClipAudio + " "+eventValue.EventName);
                                if (eventAction.ClipAudio != null)
                                    d = () => StartPlaySFX(instance as GameObject, eventAction.ClipAudio);
                                break;
                            case EventTypeEnum.STOP_PLAY_SOUND:
                                if (eventAction.ClipAudio != null)
                                    d = () => StopPlaySFX(instance as GameObject, eventAction.ClipAudio);
                                break;
                            case EventTypeEnum.VIBRATE_ALL_CONTROLLERS:
                                if (eventAction.Intensity != 0)
                                    d = () => Vibrate(eventAction.Intensity);
                                break;
                        }
                        if(d != null)
                        {
                            ((UnityEvent)(instance.GetType().GetField(eventValue.EventName).GetValue(instance))).AddListener(d);
                            var t = (UnityEvent)(instance.GetType().GetField(eventValue.EventName).GetValue(instance));
                            _actions.Add(new SAction(t,d));

                        }
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
    }

    #region Linked Methods
    public void DebugMessage(string msg)
    {
        Debug.Log($"<color=cyan>{msg}</color>");
    }
    public void PlaySFX(Clip audioClip)
    {
        _audioSource.PlayOneShot(audioClip.Audioclip, audioClip.Volume);
    }
    public void PlayRandomSFX(List<Clip> audioClips)
    {
        int randIndex = UnityEngine.Random.Range(0,audioClips.Count);
        _audioSource.PlayOneShot(audioClips[randIndex].Audioclip, audioClips[randIndex].Volume);
    }
    public void StartPlaySFX(GameObject instance, Clip audioClip)
    { 
        Debug.Log("START PlaySFX");
        AudioSource newAudioSource = _audioSourcesStorage.AddComponent<AudioSource>();
        newAudioSource.loop = true;
        newAudioSource.clip = audioClip.Audioclip;
        newAudioSource.volume = audioClip.Volume;
        newAudioSource.Play();
        _soundsPlaying.Add(new SSoundRef(newAudioSource, instance, newAudioSource.clip));
    }
    public void StopPlaySFX(GameObject instance, Clip audioClip)
    {
        Debug.Log("STOP PlaySFX");
        List<SSoundRef> _soundRefs = _soundsPlaying.Where(t => t.InstanceRef == instance).ToList();
        SSoundRef _soundRef = _soundRefs.FirstOrDefault(t => t.Clip == audioClip.Audioclip);
        if(_soundRef != null)
        {
            _soundRef.AudioSource.Stop();
            _soundsPlaying.Remove(_soundRef);
            Destroy(_soundRef.AudioSource);
        }
    }
    public void ScreenShake(float intensity)
    {
        _camera.transform.DOShakePosition(intensity);
    }
    public void Vibrate(float intensity)
    {
        _camera.transform.DOShakePosition(intensity);
        Debug.Log($"ScreenShake at intensity : {intensity}");
    }
    #endregion
}
class SSoundRef
{
    public SSoundRef(AudioSource _audioSource, GameObject _instanceRef, AudioClip _action)
    {
        audioSource = _audioSource;
        instanceRef = _instanceRef;
        clip = _action;
    }
    public AudioSource AudioSource => audioSource;
    public GameObject InstanceRef => instanceRef;
    public AudioClip Clip => clip;
    AudioSource audioSource;
    GameObject instanceRef;
    AudioClip clip;
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
    PLAY_SOUND_ONCE,
    PLAY_RANDOM_SOUND_ONCE,
    START_PLAY_SOUND,
    STOP_PLAY_SOUND,
    SCREENSHAKE,
    VIBRATE_ALL_CONTROLLERS,
}

[Serializable]
public class EventAction
{
    public EventAction() 
    {
        eventType = EventTypeEnum.NONE;
        clipAudio = new();
        clipsAudio = new();
    }

    public EventTypeEnum EventType
    {
        get { return eventType; } 
        set { eventType = value; }
    }

    public string DebugMessage
    {
        get { return debugMessage; }
        set { debugMessage = value; }
    }
    public Clip ClipAudio
    {
        get { return clipAudio; }
        set { clipAudio = value; }
    }

    public List<Clip> ClipsAudio
    {
        get { return clipsAudio; }
        set { clipsAudio = value; }
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
    private Clip clipAudio;
    [SerializeField]
    private List<Clip> clipsAudio;
    [SerializeField]
    private float intensity;

}
[Serializable]
public class Clip
{
    public AudioClip Audioclip
    {
        get { return audioclip; }
        set { audioclip = value; }
    }
    public float Volume
    {
        get { return volume; }
        set { volume = value; }
    }
    [SerializeField]
    AudioClip audioclip;
    [SerializeField]
    float volume = 1;
}
