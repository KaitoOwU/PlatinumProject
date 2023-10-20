using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemDataEditor : EditorWindow
{

    private string _name;
    private Sprite _icon;
    private GameObject _visualPrefab;

    private static int _id;

    private const string _PATH_TO_SCRIPTABLE_OBJECTS = "Assets/Scripts/Item/";

    [MenuItem("Platinum/Create new Item")]
    private static void Init()
    {
        ItemDataEditor window = GetWindowWithRect<ItemDataEditor>(new Rect(0, 0, 400, 200), true, "Item Creator");
        window.Show();
    }

    private void OnGUI()
    {
        int uniformPadding = 20;
        RectOffset padding = new RectOffset(uniformPadding, uniformPadding, uniformPadding, uniformPadding);
        Rect area = new Rect(padding.right, padding.top, position.width - (padding.right + padding.left), position.height - (padding.top + padding.bottom));
 
        GUILayout.BeginArea(area);
        {
            _name = EditorGUILayout.TextField("Nom de l'Item", _name);
            _visualPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", _visualPrefab, typeof(GameObject), false);
            _icon = (Sprite)EditorGUILayout.ObjectField("Icône de l'Item", _icon, typeof(Sprite), false);
            
            GUILayout.Space(15);
            if (GUILayout.Button("Créer"))
            {
                if (_name == string.Empty)
                {
                    EditorUtility.DisplayDialog("Erreur", "Le nom de l'Item n'est pas correct", "Super...");
                    return;
                } else if (_icon == null)
                {
                    EditorUtility.DisplayDialog("Erreur", "L'icône de l'Item n'a pas été renseigné", "Super...");
                    return;
                } else if (_visualPrefab == null)
                {
                    EditorUtility.DisplayDialog("Erreur", "Le préfab de l'Item n'a pas été renseigné", "Super...");
                    return;
                }
                else
                {

                    ItemData data = new ItemData(_id, _name, _icon, _visualPrefab);
                    
                    AssetDatabase.CreateAsset(data, _PATH_TO_SCRIPTABLE_OBJECTS + _id + "_" + _name + ".asset");
                    AssetDatabase.SaveAssets();
                    
                    EditorUtility.FocusProjectWindow();

                    Selection.activeObject = data;
                    
                    GetWindow<ItemDataEditor>().Close();
                }
            }
            
        }
        GUILayout.EndArea();
    }
}
