using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[EditorWindowTitle(title = "Item Manager", icon = "Assets/Scripts/Item/Editor/mini_3d.png")]
public class ItemManagerEditor : EditorWindow
{

    private Vector2 _scrollPos = Vector2.zero;
    private string _research = string.Empty;
    
    [MenuItem("Platinum/Manage Items")]
    private static void Init()
    {
        ItemManagerEditor window = GetWindowWithRect<ItemManagerEditor>(new Rect(0, 0, 300, 400), false);
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
        
        
        
        if (GUILayout.Button("Créer un Item", new GUIStyle(GUI.skin.button){fixedHeight = 32, fontSize = 15, fontStyle = FontStyle.Bold}))
        {
            ItemCreatorEditor.InitForCreation();
        }
        
        EditorGUILayout.Space(25);

        EditorGUILayout.BeginVertical();
        {
            if (_research != string.Empty)
            {
                GUIPrintItems(FindAllScriptableObjectsOfType<ItemData>("t:ItemData", "Assets/Scripts/Item/ItemsData").FindAll(value => value.Name.ContainsInsensitive(_research)).OrderBy(value => value.ID).ToList());
            }
            else
            {
                GUIPrintItems(FindAllScriptableObjectsOfType<ItemData>("t:ItemData", "Assets/Scripts/Item/ItemsData").OrderBy(value => value.ID).ToList());
            }
        }
        EditorGUILayout.EndVertical();

    }
    
    public static List<T> FindAllScriptableObjectsOfType<T>(string filter, string folder = "Assets")
        where T : ScriptableObject
    {
        return AssetDatabase.FindAssets(filter, new[] { folder })
            .Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
            .ToList();
    }

    private void GUIPrintItems(List<ItemData> items)
    {
        if (items.Count > 0)
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(200));
            {
                foreach (ItemData data in items)
                {
                    
                    GUILayout.Label($"({data.ID}) {data.Name}", new GUIStyle(GUI.skin.label) {fontSize = 17, fontStyle = FontStyle.Bold});

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
                                AssetDatabase.DeleteAsset("Assets/Scripts/Item/ItemsPrefabs/" + data.ID + "_" + data.Name +
                                                          "Prefab.prefab");
                                AssetDatabase.DeleteAsset("Assets/Scripts/Item/ItemsData/" + data.ID + "_" + data.Name +
                                                          "Data.asset");
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
            GUILayout.Label("Aucun Item n'a été trouvé.", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 18, fontStyle = FontStyle.Bold});
            GUI.color = Color.gray;
            GUILayout.Label("Clique sur \"Créer un Item\" pour en créer un", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 12});
            GUI.color = Color.white;
        }
    }
}
