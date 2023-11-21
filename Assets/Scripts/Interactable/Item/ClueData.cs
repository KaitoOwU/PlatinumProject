using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ClueData : PickableData
{
    [SerializeField] protected MurderScenario.SuspectDuo _suspects;
    [SerializeField, TextArea] protected string _description;
    [SerializeField] protected Sprite _sprite = null;
    public MurderScenario.SuspectDuo Suspects => _suspects;

    public void SaveData(int id, string name, GameObject prefab, MurderScenario.SuspectDuo suspects, string description, Sprite sprite = null)
    {
        _id = id;
        _name = name;
        _prefab = prefab;
        _suspects = suspects;
        _description = description;
        if (sprite is not null) _sprite = sprite;
    }
}
