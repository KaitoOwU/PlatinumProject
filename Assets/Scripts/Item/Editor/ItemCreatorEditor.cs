using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ItemCreatorEditor : EditorWindow
{
    private static PickableData _pickableModified;

    //PICKABLE DATA
    private int _id;
    private string _name = "";
    
    //ITEM DATA
    private Sprite _icon;
    
    //CLUE DATA
    private SuspectData _victim, _murderer;
    private MurderScenario _scenario;
    
    private int _toolbar;
    private bool _allConditionsOpe;

    public static void InitForCreation()
    {
        _pickableModified = null;
        ItemCreatorEditor window = GetWindowWithRect<ItemCreatorEditor>(new Rect(0, 0, 350, 500), true, "Créer un nouvel Item");
        window.Show();
    }
    
    public static void InitForModification(PickableData data)
    {
        _pickableModified = data;
        ItemCreatorEditor window = GetWindowWithRect<ItemCreatorEditor>(new Rect(0, 0, 350, 500), true, "Modifier un Item");
        window.Show();
    }

    private void CreateGUI()
    {
        if (_pickableModified != null)
        {
            _id = _pickableModified.ID;
            _name = _pickableModified.Name;
            if (_pickableModified is ItemData)
            {
                _icon = (_pickableModified as ItemData).UIIcon;
            } else if (_pickableModified is ClueData)
            {
                _victim = (_pickableModified as ClueData).Suspects.Victim;
                _murderer = (_pickableModified as ClueData).Suspects.Murderer;
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        
        if (_pickableModified == null)
        {
            GUILayout.Label("TYPE", new GUIStyle(GUI.skin.label){alignment = TextAnchor.MiddleCenter, fontSize = 20, fontStyle = FontStyle.Bold});
            _toolbar = GUILayout.Toolbar(_toolbar, new string[] { "Item", "Clue" });
        }
        else
        {
            _toolbar = _pickableModified is ItemData ? 0 : 1;
        }
        
        GUILayout.Space(10);
            
        GUILayout.Label("DATA", new GUIStyle(GUI.skin.label){alignment = TextAnchor.MiddleCenter, fontSize = 20, fontStyle = FontStyle.Bold});
        _name = EditorGUILayout.TextField("Item name", _name);
        EditorGUILayout.BeginHorizontal();
        {
            _id = EditorGUILayout.IntField("ID", _id);
            if (GUILayout.Button("Auto"))
            {
                _id = FindFirstFreeID();
            }
        }
        EditorGUILayout.EndHorizontal();
        
        switch (_toolbar)
        {
            case 0:
            {
                _icon = (Sprite)EditorGUILayout.ObjectField("UI Icon", _icon, typeof(Sprite), false);
                _allConditionsOpe = _icon != null && _name != null && IsIDFree(_id);
                break;
            }
            case 1:
            {
                EditorGUILayout.BeginHorizontal();
                {
                    _victim = (SuspectData)EditorGUILayout.ObjectField(_victim, typeof(SuspectData), false);
                    GUILayout.Label("a tué");
                    _murderer = (SuspectData)EditorGUILayout.ObjectField(_murderer, typeof(SuspectData), false);
                }
                EditorGUILayout.EndHorizontal();

                _scenario = Resources.LoadAll<MurderScenario>("Clues").ToList()
                        .FindAll(scenario => scenario.DuoSuspect.Victim == _victim)
                        .Find(scenario => scenario.DuoSuspect.Murderer == _murderer);

                if (Resources.LoadAll<MurderScenario>("Clues").Length == 0)
                {
                    EditorGUILayout.HelpBox("Les scénarios n'ont pas été générés, générez les dans le Scenario Manager", MessageType.Error);
                    _allConditionsOpe = false;
                } else if (_scenario == null)
                {
                    EditorGUILayout.HelpBox("Aucun scénario ne correspond à ce duo", MessageType.Warning);
                    _allConditionsOpe = false;
                }
                else
                {
                    EditorGUILayout.HelpBox($"{_scenario.name} a été trouvé", MessageType.Info);
                    _allConditionsOpe = _name != null && IsIDFree(_id);
                }
                break;
            }
        }
        
        if (_pickableModified == null)
        {
            GUILayout.Space(10);
            
            GUI.enabled = _allConditionsOpe;
            if (GUILayout.Button("Créer l'Item"))
            {
                if (_toolbar == 0)
                {
                    //COPIE DU PREFAB ORIGINEL
                    string name = "Item" + _id + "_" + Regex.Replace(_name, "[^0-9A-Za-z_-]", "");
                    AssetDatabase.CopyAsset("Assets/Scripts/Item/Editor/ItemPrefab -- DO NOT TOUCH --.prefab",
                        $"Assets/Resources/Item/ItemsPrefabs/{name}.prefab");
                    GameObject prefab =
                        AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Resources/Item/ItemsPrefabs/{name}.prefab");
                    
                    //CREATION DU SCRIPTABLE
                    ItemData item = CreateInstance<ItemData>();
                    AssetDatabase.CreateAsset(item, $"Assets/Resources/Item/ItemsData/{name}.asset");
                    item.SaveData(_id, _name, prefab, _icon);
                } else if (_toolbar == 1)
                {
                    //COPIE DU PREFAB ORIGINEL
                    string name = "Clue" + _id + "_" + Regex.Replace(_name, "[^0-9A-Za-z_-]", "");
                    AssetDatabase.CopyAsset("Assets/Scripts/Item/Editor/CluePrefab -- DO NOT TOUCH --.prefab",
                        $"Assets/Resources/Clues/CluePrefab/{name}.prefab");
                    GameObject prefab =
                        AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Resources/Clues/CluePrefab/{name}.prefab");
                    
                    //CREATION DU SCRIPTABLE
                    ClueData clue = CreateInstance<ClueData>();
                    AssetDatabase.CreateAsset(clue, $"Assets/Resources/Clues/ClueData/{name}.asset");
                    clue.SaveData(_id, _name, prefab, new MurderScenario.SuspectDuo(_victim, _murderer));
                    
                    //APPLICATION AU SCENARIO DEFINI
                    _scenario.Clues.Add(prefab.GetComponent<Clue>());
                }

                Close();
            }
            GUI.enabled = true;
        }
        else
        {
            if (GUILayout.Button("Modifier l'Item"))
            {
                if (_pickableModified is ItemData)
                {
                    //RENOMMAGE DU PREFAB
                    string name = "Item" + _pickableModified.ID + "_" +
                                  Regex.Replace(_pickableModified.Name, "[^0-9A-Za-z_-]", "");
                    string newName = "Item" + _id + "_" + Regex.Replace(_name, "[^0-9A-Za-z_-]", "");
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_pickableModified),
                        newName + ".prefab");

                    //MODIFICATION DU SCRIPTABLE
                    ((ItemData)_pickableModified).SaveData(_id, _name, _pickableModified.Prefab, _icon);
                }
                else
                {
                    //RENOMMAGE DU PREFAB
                    string name = "Clue" + _pickableModified.ID + "_" +
                                  Regex.Replace(_pickableModified.Name, "[^0-9A-Za-z_-]", "");
                    string newName = "Clue" + _id + "_" + Regex.Replace(_name, "[^0-9A-Za-z_-]", "");
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_pickableModified.Prefab.GetInstanceID()),
                        newName + ".prefab");

                    //MODIFICATION DU SCRIPTABLE
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_pickableModified),
                        newName + ".asset");
                    ((ClueData)_pickableModified).SaveData(_id, _name, _pickableModified.Prefab,
                        new MurderScenario.SuspectDuo(_victim, _murderer));
                }

                Close();
            }
        }

        if (!_allConditionsOpe)
        {
            _PrintHelpboxProblems();
        }
    }

    private static int FindFirstFreeID()
    {
        var type = ItemManagerEditor.FindAllScriptableObjectsOfType<PickableData>("t:PickableData");

        if (type.Count == 0)
            return 0;
        
        for (var index = 0; index < type.Count; index++)
        {
            PickableData pickableData = type[index];
            if (pickableData.ID != index)
                return index;
        }

        return type[^1].ID + 1;
    }

    private static bool IsIDFree(int idToFind)
    {
        var type = ItemManagerEditor.FindAllScriptableObjectsOfType<PickableData>("t:PickableData");

        if(_pickableModified is null)
            return type.Find(value => value.ID == idToFind) == null;
        else
            return type.Find(value => value.ID == idToFind && value.ID != _pickableModified.ID) == null;
    }

    private void _PrintHelpboxProblems()
    {
        StringBuilder str = new();
        
        if (!IsIDFree(_id)) str.Append($"Cet ID ({_id}) est déjà pris");
        if (_name == string.Empty || _name == null) str.Append("\nLe nom ne peut pas être nul");
        
        if (_toolbar == 0)
        {
            if (_icon == null) str.Append("\nL'icône ne peut pas être nul");
        } else if (_toolbar == 1)
        {
            if (_scenario == null) str.Append("\nUn scénario est nécéssaire pour créer l'Indice");
        }
        
        if(str.ToString() != "")
            EditorGUILayout.HelpBox(str.ToString(), MessageType.Error);
    }
}
