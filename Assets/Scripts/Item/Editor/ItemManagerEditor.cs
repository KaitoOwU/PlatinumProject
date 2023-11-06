using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

[EditorWindowTitle(title = "Item Manager", icon = "Assets/Scripts/Item/Editor/mini_3d.png")]
public class ItemManagerEditor : EditorWindow
{

    private Vector2 _scrollPos = Vector2.zero;
    private string _research = string.Empty;
    private int _toolbar;
    
    [MenuItem("Platinum/Manage Items")]
    public static void Init()
    {
        ItemManagerEditor window = GetWindowWithRect<ItemManagerEditor>(new Rect(0, 0, 500, 700), false);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);
        
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Scripts/Item/Editor/3d.png"), new GUIStyle(GUI.skin.label) {fixedWidth = 64, fixedHeight = 64});
            GUILayout.Label("Item Manager", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 30, fontStyle = FontStyle.Bold, fixedHeight = 64});
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(20);
        
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Rechercher");
            _research = EditorGUILayout.TextField(_research);
        }
        EditorGUILayout.EndHorizontal();



        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Créer un Item/Indice", new GUIStyle(GUI.skin.button){fixedHeight = 32, fontSize = 15, fontStyle = FontStyle.Bold}))
            {
                ItemCreatorEditor.InitForCreation();
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(10);
        _toolbar = GUILayout.Toolbar(_toolbar, new string[] { "Tout", "Items", "Indices" });
        EditorGUILayout.Space(20);

        switch (_toolbar)
        {
            case 0:
            {
                EditorGUILayout.BeginVertical();
                {
                    if (_research != string.Empty)
                    {
                        GUIPrintItems(FindAllScriptableObjectsOfType<PickableData>("t:PickableData").FindAll(value => value.Name.ContainsInsensitive(_research)).OrderBy(value => value.ID).ToList());
                    }
                    else
                    {
                        GUIPrintItems(FindAllScriptableObjectsOfType<PickableData>("t:PickableData").OrderBy(value => value.ID).ToList());
                    }
                }
                EditorGUILayout.EndVertical();
                break;
            }

            case 1:
            {
                EditorGUILayout.BeginVertical();
                {
                    if (_research != string.Empty)
                    {
                        GUIPrintItems(FindAllScriptableObjectsOfType<PickableData>("t:ItemData").FindAll(value => value.Name.ContainsInsensitive(_research)).OrderBy(value => value.ID).ToList());
                    }
                    else
                    {
                        GUIPrintItems(FindAllScriptableObjectsOfType<PickableData>("t:ItemData").OrderBy(value => value.ID).ToList());
                    }
                }
                EditorGUILayout.EndVertical();
                break;
            }
            
            case 2:
            {
                EditorGUILayout.BeginVertical();
                {
                    if (_research != string.Empty)
                    {
                        GUIPrintItems(FindAllScriptableObjectsOfType<PickableData>("t:ClueData").FindAll(value => value.Name.ContainsInsensitive(_research)).OrderBy(value => value.ID).ToList());
                    }
                    else
                    {
                        GUIPrintItems(FindAllScriptableObjectsOfType<PickableData>("t:ClueData").OrderBy(value => value.ID).ToList());
                    }
                }
                EditorGUILayout.EndVertical();
                break;
            }
        }

    }

    private void GUIPrintItems(List<PickableData> items)
    {
        if (items.Count > 0)
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(500));
            {
                foreach (PickableData data in items)
                {
                    
                    GUILayout.Label($"{(data is ItemData ? "<color=#f170ff>Item</color>" : "<color=#70bfff>Clue</color>")} • ({data.ID}) {data.Name}", new GUIStyle(GUI.skin.label) {fontSize = 17, fontStyle = FontStyle.Bold, richText = true});

                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Prefab"))
                        {
                            Selection.activeObject = data.Prefab;
                            EditorGUIUtility.PingObject(Selection.activeObject);
                        }
                        
                        if (GUILayout.Button("Modifier"))
                        {
                            ItemCreatorEditor.InitForModification(data);
                        }

                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("Supprimer"))
                        {
                            if(EditorUtility.DisplayDialog("Supprimer Item ?", $"Tu es sûr de vouloir supprimer l'Item : \"{data.Name}\" \nCela supprimera également le Prefab lié à l'Item !", "Oui", "Non"))
                            {
                                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(data.Prefab.GetInstanceID()));
                                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(data));
                            }
                        }
                        GUI.backgroundColor = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.Space(10);
                }
            }
            EditorGUILayout.EndScrollView();
        }
        else
        {
            GUI.color = Color.red;
            if(_toolbar == 0) GUILayout.Label("Aucun Item ni Indice n'a été trouvé.", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 18, fontStyle = FontStyle.Bold});
            else if(_toolbar == 1) GUILayout.Label("Aucun Item n'a été trouvé.", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 18, fontStyle = FontStyle.Bold});
            else if(_toolbar == 2) GUILayout.Label("Aucun Indice n'a été trouvé.", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 18, fontStyle = FontStyle.Bold});
            GUI.color = Color.gray;
            GUILayout.Label("Clique sur \"Créer un Item\" pour en créer un", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 12});
            GUI.color = Color.white;
        }
    }

    public static List<T> FindAllScriptableObjectsOfType<T>(string filter, string folder = "Assets")
        where T : ScriptableObject
    {
        return AssetDatabase.FindAssets(filter, new[] { folder })
            .Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
            .ToList();
    }
}
