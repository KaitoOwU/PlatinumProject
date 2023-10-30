using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnityEventData))]
public class EventInspector : Editor
{
    public override void OnInspectorGUI()
    {
        UnityEventData data = (UnityEventData)target;

        GUILayout.Label($"Events Data n°{data.name[^1]}", new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold });
        //GUI.enabled = false;
        //GUIStyle scriptNameStyle = GUI.skin.label;
        //scriptNameStyle.normal.textColor = new Color(0.1f, 01f, 1);

        foreach (var script in data.DataBase)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); //Line

            GUILayout.Label(script.ScriptName, new GUIStyle(GUI.skin.label) { fontSize = 17, fontStyle = FontStyle.Bold, normal = new GUIStyleState() {textColor = new Color(0.5f, 0.7f, 0.9f) } });

            foreach (var eventInfo in script.Events)
            {
                GUILayout.Space(10);    
                GUILayout.Label(eventInfo.EventName, new GUIStyle(GUI.skin.label){ fontSize = 13, fontStyle = FontStyle.Bold});

                
                eventInfo.EventAction.EventType = (EventTypeEnum)EditorGUILayout.EnumPopup(eventInfo.EventAction.EventType);
                switch (eventInfo.EventAction.EventType)
                {
                    case EventTypeEnum.NONE: break;
                    case EventTypeEnum.DEBUG:
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Debug Message", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic, normal = new GUIStyleState() { textColor = new Color(0.9f, 0.6f, 0.8f) } });
                        eventInfo.EventAction.DebugMessage = GUILayout.TextField(eventInfo.EventAction.DebugMessage);
                        EditorGUILayout.EndHorizontal();
                        break;
                    case EventTypeEnum.PLAY_SOUND:
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("SFX", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic, normal = new GUIStyleState() { textColor = new Color(0.9f, 0.6f, 0.8f) } });
                        eventInfo.EventAction.Clip = (AudioClip)EditorGUILayout.ObjectField(eventInfo.EventAction.Clip, typeof(AudioClip), true);
                        EditorGUILayout.EndHorizontal();
                        break;
                    case EventTypeEnum.SCREENSHAKE:
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Screenshake Intensity", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic, normal = new GUIStyleState() { textColor = new Color(0.9f, 0.6f, 0.8f) } });
                        eventInfo.EventAction.Intensity = EditorGUILayout.FloatField(eventInfo.EventAction.Intensity);
                        EditorGUILayout.EndHorizontal();
                        break;
                }
            }
        }
    }
}
