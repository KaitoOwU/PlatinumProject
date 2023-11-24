using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnityEventData : ScriptableObject
{
    public SerializedProperty serializedProperty => new UnityEditor.SerializedObject(this).FindProperty("_database");

    [SerializeField]
    private List<ScriptEventInfo> _database;

    [SerializeField]
    public List<ScriptEventInfo> DataBase
    {
        get { return _database; }
        set { _database = value; }
    }
}
