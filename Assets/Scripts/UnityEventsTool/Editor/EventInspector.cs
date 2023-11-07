using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Linq;

[CustomEditor(typeof(UnityEventData))]
public class EventInspector : Editor
{
    public Font font;

    public override void OnInspectorGUI()
    {
        UnityEventData data = (UnityEventData)target;
        GUIStyle buttonStyle = GUI.skin.button;
        buttonStyle.fontSize = 13;
        GUILayout.Label($"Events Data n°{data.name[^1]}", new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold });
        //GUI.enabled = false;
        //GUIStyle scriptNameStyle = GUI.skin.label;
        //scriptNameStyle.normal.textColor = new Color(0.1f, 01f, 1);
        GUI.skin.font = font;


        foreach (var script in data.DataBase)
        {
            DrawUILine(new Color(0.8f, 0.8f, 0.8f), 2, 0);

            //EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); //Line
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
                    eventInfo.EventActions.Add(new());
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
                        case EventTypeEnum.PLAY_SOUND:
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("SFX", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic, normal = new GUIStyleState() { textColor = new Color(1, 0.8f, 0.4f) } });
                            eventAction.Clip = (AudioClip)EditorGUILayout.ObjectField(eventAction.Clip, typeof(AudioClip), true);
                            EditorGUILayout.EndHorizontal();
                            break;
                        case EventTypeEnum.SCREENSHAKE:
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("Screenshake Intensity", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic, normal = new GUIStyleState() { textColor = new Color(0.9f, 0.6f, 0.8f) } });
                            eventAction.Intensity = EditorGUILayout.FloatField(eventAction.Intensity);
                            EditorGUILayout.EndHorizontal();
                            break;
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUI.backgroundColor = new Color(1f, 0.4f, 0.6f);
                    if (GUILayout.Button("Remove Action", buttonStyle, GUILayout.Width(150)))
                    {
                        eventInfo.EventActions.Remove(eventAction);
                        EditorUtility.SetDirty(target);
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();

                    //DrawUILine(new Color(0.6f,0.6f,0.6f), 1, 20);
                        //EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); //Line
                }

                GUILayout.EndVertical();
                //GUILayout.EndHorizontal();
            }
        }
    }
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
