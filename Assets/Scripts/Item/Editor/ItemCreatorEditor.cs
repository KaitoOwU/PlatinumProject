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

    private bool _isThrowable;
    private GenerationZone _generationZone;

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
            _isThrowable = _itemModified.IsThrowable;
            _generationZone = _itemModified.GenerationZoneAsEnum;
        }
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Data", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 20, fontStyle = FontStyle.Bold}, GUILayout.ExpandWidth(true));
        GUILayout.Space(10);
        
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("ID de l'Item");
            _id = EditorGUILayout.IntField(Mathf.Abs(_id));
            if (GUILayout.Button("Auto"))
            {
                _id = FindFirstFreeID();
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Nom de l'Item");
            _name = EditorGUILayout.TextField(_name);
        }
        EditorGUILayout.EndHorizontal();
        
        _icon = (Sprite)EditorGUILayout.ObjectField("Icône de l'Item (UI)", _icon, typeof(Sprite), false);
        
        GUILayout.Space(10);
        GUILayout.Label("Behaviour", new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 20, fontStyle = FontStyle.Bold}, GUILayout.ExpandWidth(true));
        GUILayout.Space(10);
        
        _isThrowable = EditorGUILayout.Toggle("Is Throwable ?", _isThrowable);
        _generationZone = (GenerationZone)EditorGUILayout.EnumFlagsField("Generation Zones", _generationZone);
        
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
                    AssetDatabase.CopyAsset("Assets/Scripts/Item/Editor/ItemPrefab -- DO NOT TOUCH --.prefab", "Assets/Scripts/Item/ItemsPrefabs/" + _id + "_" + _name + "Prefab.prefab");
                    GameObject prefab =
                        AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scripts/Item/ItemsPrefabs/" + _id + "_" + _name +
                                                                  "Prefab.prefab");
                
                    ItemData data = CreateInstance<ItemData>();
                    data.SaveData(_id, _name, _icon, prefab, _isThrowable, _generationZone);
                    AssetDatabase.CreateAsset(data, "Assets/Scripts/Item/ItemsData/" + _id + "_" + _name +
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
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_itemModified.Prefab), _itemModified.ID + "_" + _itemModified.Name + "Prefab.prefab");
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_itemModified), _itemModified.ID + "_" + _itemModified.Name + "Data.asset");
                    
                    _itemModified.SaveData(_id, _name, _icon, _itemModified.Prefab, _isThrowable, _generationZone);
                    Close();
                }
            }
        }
    }

    private static int FindFirstFreeID()
    {
        var type = ItemManagerEditor.FindAllScriptableObjectsOfType<ItemData>("t:ItemData",
            "Assets/Scripts/Item/ItemsData");

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
            "Assets/Scripts/Item/ItemsData");

        return type.Find(value => value.ID == idToFind) == null;
    }
}
