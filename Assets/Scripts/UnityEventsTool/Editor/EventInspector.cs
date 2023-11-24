using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.PackageManager.UI;

[CustomEditor(typeof(UnityEventData))]
public class EventInspector : Editor
{
    public Font font;

    #region Draw Inspector

    public override void OnInspectorGUI()
    {
        UnityEventData data = (UnityEventData)target;
        GUIStyle buttonStyle = GUI.skin.button;
        buttonStyle.fontSize = 13;
        GUILayout.Label($"Events Data n°{data.name[^1]}", new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold });

        GUI.skin.font = font;

        serializedObject.Update();


        foreach (var script in data.DataBase)
        {
            DrawUILine(new Color(0.8f, 0.8f, 0.8f), 2, 0);
            GUILayout.Space(5);
            GUILayout.Label(script.ScriptName, new GUIStyle(GUI.skin.label) { fontSize = 17, normal = new GUIStyleState() {textColor = new Color(0.5f, 0.7f, 0.9f) } });

            foreach (var eventInfo in script.Events)
            {

                GUILayout.Space(10);
                GUI.backgroundColor = new Color(0.6f, 0.6f, 0.6f);
                GUILayout.BeginVertical("GroupBox");


                GUILayout.Label(SplitName(eventInfo.EventName), new GUIStyle(GUI.skin.label) { fontSize = 13});

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = new Color(0.8f, 1f, 0.7f);
                if (GUILayout.Button("Add Action when event is called", buttonStyle, GUILayout.Width(250)))
                {
                    eventInfo.EventActions.Add(new());
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                for(int i = 0; i < eventInfo.EventActions.Count; i++)
                {
                    var eventAction = eventInfo.EventActions[i];
                    GUILayout.Space(5);
                    GUI.backgroundColor = new Color(0.9f, 0.9f, 0.9f);
                    GUILayout.BeginVertical("HelpBox");

                    eventAction.EventType = (EventTypeEnum)EditorGUILayout.EnumPopup(eventAction.EventType);
                    switch (eventAction.EventType)
                    {
                        case EventTypeEnum.NONE: break;
                        case EventTypeEnum.DEBUG:
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Debug Message", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic, normal = new GUIStyleState() { textColor = new Color(0.7f, 1f, 0.6f) } });
                            eventAction.DebugMessage = GUILayout.TextField(eventAction.DebugMessage);
                            EditorGUILayout.EndHorizontal();
                            break;
                        case EventTypeEnum.PLAY_SOUND_ONCE:
                        case EventTypeEnum.START_PLAY_SOUND:
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("SFX", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic, normal = new GUIStyleState() { textColor = new Color(1, 0.8f, 0.4f) } });

                            EditorGUILayout.BeginHorizontal();
                            GUI.backgroundColor = new Color(1f, 1f, 1f);
                            eventAction.ClipAudio.Audioclip = EditorGUILayout.ObjectField(eventAction.ClipAudio.Audioclip, typeof(AudioClip), true) as AudioClip;
                            eventAction.ClipAudio.Volume = EditorGUILayout.Slider(eventAction.ClipAudio.Volume, 0, 1, GUILayout.Width(150));
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.EndHorizontal();
                            break;
                        case EventTypeEnum.STOP_PLAY_SOUND:
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("SFX", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic, normal = new GUIStyleState() { textColor = new Color(1, 0.8f, 0.4f) } });

                            EditorGUILayout.BeginHorizontal();
                            GUI.backgroundColor = new Color(1f, 1f, 1f);
                            eventAction.ClipAudio.Audioclip = EditorGUILayout.ObjectField(eventAction.ClipAudio.Audioclip, typeof(AudioClip), true) as AudioClip;
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.EndHorizontal();
                            break;
                        case EventTypeEnum.PLAY_RANDOM_SOUND_ONCE:                                
                            if (GUILayout.Button("Add SFX", buttonStyle, GUILayout.Width(150)))
                            {
                                EditorUtility.SetDirty(data);
                                eventInfo.EventActions[i].ClipsAudio.Add(new());
                                AssetDatabase.SaveAssetIfDirty(data);
                            }
                            for(int c = 0; c< eventAction.ClipsAudio.Count; c++)
                            {
                                var clip = eventAction.ClipsAudio[c];
                                GUI.backgroundColor = new Color(0f, 0f, 0f);
                                GUILayout.BeginVertical("HelpBox");


                                EditorGUILayout.BeginHorizontal();
                                GUILayout.Label("SFX", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic, normal = new GUIStyleState() { textColor = new Color(1, 0.8f, 0.4f) } });
                                EditorGUILayout.BeginHorizontal();
                                GUI.backgroundColor = new Color(1f, 1f, 1f);
                                eventAction.ClipsAudio[c].Audioclip = (AudioClip)EditorGUILayout.ObjectField(eventAction.ClipsAudio[c].Audioclip, typeof(AudioClip), true);
                                eventAction.ClipsAudio[c].Volume = EditorGUILayout.Slider(clip.Volume, 0, 1, GUILayout.Width(150));
                                EditorGUILayout.EndHorizontal();


                                GUI.backgroundColor = new Color(1f, 0.5f, 0.6f);
                                if (GUILayout.Button("X", buttonStyle, GUILayout.Width(50)))
                                {
                                    EditorUtility.SetDirty(data);
                                    eventAction.ClipsAudio.Remove(clip);
                                    AssetDatabase.SaveAssetIfDirty(data);
                                }
                                EditorGUILayout.EndHorizontal();


                                GUILayout.EndVertical();
                            }
                            break;
                        case EventTypeEnum.SCREENSHAKE:
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Screenshake Intensity", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic, normal = new GUIStyleState() { textColor = new Color(0.9f, 0.6f, 0.8f) } });
                            eventAction.Intensity = EditorGUILayout.Slider(eventAction.Intensity, 0, 1, GUILayout.Width(200));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Vibration Duration", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic, normal = new GUIStyleState() { textColor = new Color(0.6f, 0.9f, 0.8f) } });
                            eventAction.Duration = EditorGUILayout.Slider(eventAction.Duration, 0, 5, GUILayout.Width(200));
                            EditorGUILayout.EndHorizontal();
                            break;
                        case EventTypeEnum.VIBRATE_ALL_CONTROLLERS:
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Left Motor Intensity", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic, normal = new GUIStyleState() { textColor = new Color(0.9f, 0.6f, 0.8f) } });
                            eventAction.Intensity = EditorGUILayout.Slider(eventAction.Intensity, 0, 1, GUILayout.Width(200));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Right Motor Intensity", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic, normal = new GUIStyleState() { textColor = new Color(0.9f, 0.6f, 0.8f) } });
                            eventAction.IntensityRight = EditorGUILayout.Slider(eventAction.IntensityRight, 0, 1, GUILayout.Width(200));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Vibration Duration", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic, normal = new GUIStyleState() { textColor = new Color(0.6f, 0.9f, 0.8f) } });
                            eventAction.Duration = EditorGUILayout.Slider(eventAction.Duration, 0, 5, GUILayout.Width(200));
                            EditorGUILayout.EndHorizontal();
                            break;
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUI.backgroundColor = new Color(1f, 0.4f, 0.6f);
                    if (GUILayout.Button("Remove Action", buttonStyle, GUILayout.Width(150)))
                    {
                        eventInfo.EventActions.Remove(eventAction);
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                }

                GUILayout.EndVertical();
            }
        }
        if (GUI.changed)
            AssetDatabase.SaveAssetIfDirty(target);

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    #endregion

    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }

    public static string SplitName(string str)
    {
        string[] allWordsInName = Regex.Split(str, @"(?<!^)(?=[A-Z])");
        string name = "";
        foreach (string word in allWordsInName)
        {
            name += word;
            name += " ";
        }
        return name;
    }
}
