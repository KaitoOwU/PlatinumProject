using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DG.Tweening;
using Unity.VisualScripting.ReorderableList;
using UnityEditor;
using UnityEngine;

[EditorWindowTitle(title = "Scenario Manager", icon = "Assets/Scripts/Clues/Editor/scenario (1).png")]
public class ScenarioEditorWindow : EditorWindow
{
    [SerializeField] private Font _font;
    private List<MurderScenario> _murderScenarios => Resources.LoadAll<MurderScenario>("Clues").ToList();
    private SuspectData _victim, _murderer;
    
    private Vector2 _scrollPos = Vector2.zero;

    [MenuItem("Platinum/Manage Scenarios")]
    public static void Init()
    {
        ScenarioEditorWindow window = GetWindowWithRect<ScenarioEditorWindow>(new Rect(0, 0, 500, 700), false);
        window.Show();
    }

    private void OnGUI()
    {
        //GUI.skin.font = _font;
        EditorGUILayout.Space(10);
        
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Scripts/Clues/Editor/scenario.png"), new GUIStyle(GUI.skin.label) {fixedWidth = 64, fixedHeight = 64});
            GUILayout.Label("Scenario Manager", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 30, fontStyle = FontStyle.Bold, fixedHeight = 64});
        }
        EditorGUILayout.EndHorizontal();
        
        if (_murderScenarios.Count == 0)
        {
            GUILayout.Space(20);
            GUI.color = new Color(1f, .3f, .3f);
            GUILayout.Label("Aucun Scénario détecté", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 25});
            GUI.color = Color.white;
            
            GUILayout.Space(10);
            GUI.backgroundColor = Color.magenta;
            if (GUILayout.Button("Générer", new GUIStyle(GUI.skin.button) {margin = new RectOffset(100, 100, 0, 0), padding = new RectOffset(0, 0, 10, 10), alignment = TextAnchor.MiddleCenter, fontSize = 20, fontStyle = FontStyle.Bold}))
            {
                GenerateScenarios();
            }

            GUI.backgroundColor = Color.white;
        } else if (_murderScenarios.Count < 16)
        {
            GUILayout.Space(20);
            GUI.color = new Color(1f, .3f, .3f);
            GUILayout.Label("Scénario(s) manquant(s) detecté(s)", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 25});
            GUI.color = Color.white;
            
            GUILayout.Space(10);
            GUI.backgroundColor = Color.magenta;
            if (GUILayout.Button("Regenérer", new GUIStyle(GUI.skin.button) {margin = new RectOffset(100, 100, 0, 0), padding = new RectOffset(0, 0, 10, 10), alignment = TextAnchor.MiddleCenter, fontSize = 20, fontStyle = FontStyle.Bold}))
            {
                GenerateScenarios();
            }

            GUI.backgroundColor = Color.white;
        }
        else
        {
            GUILayout.Space(10);
            
            EditorGUILayout.BeginHorizontal();
            {
                _murderer = (SuspectData) EditorGUILayout.ObjectField(_murderer, typeof(SuspectData), false);
                GUILayout.Label(" a tué ", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 15});
                _victim = (SuspectData) EditorGUILayout.ObjectField(_victim, typeof(SuspectData), false);
            }
            EditorGUILayout.EndHorizontal();

            MurderScenario scenario = _murderScenarios.
                Find(scenario => scenario.DuoSuspect.Murderer == _murderer && scenario.DuoSuspect.Victim == _victim);
            if (_victim == null || _murderer == null || scenario == null)
            {
                EditorGUILayout.HelpBox("Aucun scénario ne correspond à ces paramètres", MessageType.Warning, true);
            }
            else
            {
                EditorGUILayout.HelpBox($"{scenario.name} trouvé !", MessageType.Info, true);
                
                GUILayout.Space(30);
                EditorGUILayout.BeginHorizontal();
                {
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("+", new GUIStyle(GUI.skin.button){fixedHeight = 20, fixedWidth = 20, fontSize = 20, alignment = TextAnchor.MiddleCenter}))
                    {
                        scenario.Clues.Add(null);
                    }
                    GUI.backgroundColor = Color.red;
                    if (scenario.Clues.Count > 0 && GUILayout.Button("-", new GUIStyle(GUI.skin.button){fixedHeight = 20, fixedWidth = 20, fontSize = 20, alignment = TextAnchor.MiddleCenter}))
                    {
                        scenario.Clues.Remove(scenario.Clues[^1]);
                    }
                    GUI.backgroundColor = Color.white;
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10);
                if (scenario.Clues.Count > 0)
                {
                    _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(400));
                    {
                        for (var index = 0; index < scenario.Clues.Count; index++)
                        {
                            Clue clue = scenario.Clues[index];
                            GUILayout.Label("Indice n°" + index, new GUIStyle(GUI.skin.label){fontSize = 17, fontStyle = FontStyle.Bold});
                            scenario.Clues[index] = (Clue)EditorGUILayout.ObjectField(clue, typeof(Clue), false);
                            GUILayout.Space(10);
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    GUI.color = new Color(1f, .3f, .3f);
                    GUILayout.Label("Aucun indice associé", new GUIStyle(GUI.skin.label){fontStyle = FontStyle.Bold, fontSize = 20, alignment = TextAnchor.MiddleCenter});
                    GUI.color = Color.white;
                }
            }
        }
    }

    private void GenerateScenarios()
    {
        _murderScenarios.ForEach(ms => AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(ms)));
        
        List<SuspectData> _suspects = ItemManagerEditor.FindAllScriptableObjectsOfType<SuspectData>("t:SuspectData", "Assets/Datas/Suspects").ToList();
        int i = 0;
        
        EditorUtility.DisplayProgressBar("Création des Intéractions", "Démarrage", 0f);
        int total = (int)Math.Pow(_suspects.Count, 2);
        
        foreach (SuspectData murderer in _suspects)
        {
            foreach (SuspectData victim in _suspects)
            {
                EditorUtility.DisplayProgressBar("Création des Interactions", "Création de \"Interaction " + i + "\"", ((float)i+1f)/(float)total);
                    
                MurderScenario scenario = CreateInstance<MurderScenario>();
                scenario.SaveData(new MurderScenario.SuspectDuo(victim, murderer));
                AssetDatabase.CreateAsset(scenario, "Assets/Resources/Clues/Interactions/Interaction" + i + ".asset");
                AssetDatabase.SaveAssets();
                i++;
            }
        }
        EditorUtility.ClearProgressBar();
        OnGUI();
    }
}
