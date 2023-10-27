using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ItemCreatorEditor : EditorWindow
{
    private static ItemData _itemModified;

    private int _id;
    private string _name;
    private Sprite _icon;

    private bool _isClue;

    public static void InitForCreation()
    {
        _itemModified = null;
        ItemCreatorEditor window = GetWindowWithRect<ItemCreatorEditor>(new Rect(0, 0, 250, 450), true, "Créer un nouvel Item");
        window.Show();
    }
    
    public static void InitForModification(ItemData data)
    {
        _itemModified = data;
        ItemCreatorEditor window = GetWindowWithRect<ItemCreatorEditor>(new Rect(0, 0, 250, 450), true, "Modifier un Item");
        window.Show();
    }

    private void CreateGUI()
    {
        if (_itemModified != null)
        {
            _id = _itemModified.ID;
            _name = _itemModified.Name;
            _icon = _itemModified.Icon;
            _isClue = _itemModified.IsClue;
        }
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("DATA", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 20, fontStyle = FontStyle.Bold}, GUILayout.ExpandWidth(true));
        GUILayout.Space(20);
        
        EditorGUILayout.BeginHorizontal();
        {
            if (_itemModified != null)
                GUI.enabled = false;
            
            GUILayout.Label("ID de l'Item");
            
            
            _id = EditorGUILayout.IntField(Mathf.Abs(_id));
            if (GUILayout.Button("Auto"))
            {
                _id = FindFirstFreeID();
            }
            GUI.enabled = true;
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Nom de l'Item");
            _name = EditorGUILayout.TextField(_name);
        }
        EditorGUILayout.EndHorizontal();
        
        _icon = (Sprite)EditorGUILayout.ObjectField("Icône de l'Item (UI)", _icon, typeof(Sprite), false);
        
        EditorGUILayout.Separator();
        
        GUILayout.Space(10);
        GUILayout.Label("BEHAVIOUR", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 20, fontStyle = FontStyle.Bold}, GUILayout.ExpandWidth(true));
        GUILayout.Space(20);
        
        _isClue = EditorGUILayout.Toggle("Is Clue ?", _isClue);
        
        GUILayout.Space(10);

        if (_itemModified == null)
        {
            if (GUILayout.Button("Créer l'item"))
            {
                if (!IsIDFree(_id))
                {
                    EditorUtility.DisplayDialog("Erreur : ID déjà utilisé", "Cet ID est déjà utilisé\nUtilise le bouton \"Auto\" pour trouver le premier ID libre", "Ok");
                }
                else if (_name == string.Empty)
                {
                    EditorUtility.DisplayDialog("Erreur : nom invalide", "Le nom que tu as donné à l'Item est invalide.", "Ok");
                } else if (_icon == null)
                {
                    EditorUtility.DisplayDialog("Erreur : icône manquant", "Tu n'as pas donné d'icône à ton Item.", "Ok");
                }
                else
                {
                    AssetDatabase.CopyAsset("Assets/Scripts/Item/Editor/ItemPrefab -- DO NOT TOUCH --.prefab", "Assets/Resources/Item/ItemsPrefabs/" + _id + "_" + _name + "Prefab.prefab");
                    GameObject prefab =
                        AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Item/ItemsPrefabs/" + _id + "_" + _name +
                                                                  "Prefab.prefab");
                
                    ItemData data = CreateInstance<ItemData>();
                    data.SaveData(_id, _name, _icon, prefab, _isClue);
                    
                    AssetDatabase.CreateAsset(data, "Assets/Resources/Item/ItemsData/" + _id + "_" + _name +
                                                    "Data.asset");

                    prefab.GetComponent<Item>().ItemData = data;
                
                    AssetDatabase.SaveAssets();
                    
                    Close();
                }
            }
        }
        else
        {
            if (GUILayout.Button("Sauvegarder l'item"))
            {
                if (_name == string.Empty)
                {
                    EditorUtility.DisplayDialog("Erreur : nom invalide", "Le nom que tu as donné à l'Item est invalide.", "Ok");
                } else if (_icon == null)
                {
                    EditorUtility.DisplayDialog("Erreur : icône manquant", "Tu n'as pas donné d'icône à ton Item.", "Ok");
                }
                else
                {
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_itemModified.Prefab), _id + "_" + _name + "Prefab.prefab");
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_itemModified), _id + "_" + _name + "Data.asset");
                    
                    _itemModified.SaveData(_id, _name, _icon, _itemModified.Prefab, _isClue);
                    Close();
                }
            }
        }
    }

    private static int FindFirstFreeID()
    {
        var type = ItemManagerEditor.FindAllScriptableObjectsOfType<ItemData>("t:ItemData",
            "Assets/Resources/Item/ItemsData");

        if (type.Count == 0)
            return 0;
        
        for (var index = 0; index < type.Count; index++)
        {
            ItemData itemData = type[index];
            if (itemData.ID != index)
                return index;
        }

        return type[^1].ID + 1;
    }

    private static bool IsIDFree(int idToFind)
    {
        var type = ItemManagerEditor.FindAllScriptableObjectsOfType<ItemData>("t:ItemData",
            "Assets/Resources/Item/ItemsData");

        return type.Find(value => value.ID == idToFind) == null;
    }
}
