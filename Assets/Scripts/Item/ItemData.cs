using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : ScriptableObject
{
    
    private string _name;
    private int _id;
    [SerializeField] private Sprite _icon;
    [SerializeField] private GameObject _prefab;

    [SerializeField] private bool _isClue;
    
    public string Name => _name;
    public int ID => _id;
    public Sprite Icon => _icon;
    public GameObject Prefab => _prefab;
    public bool IsClue => _isClue;

    public void SaveData(int id, string name, Sprite icon, GameObject prefab, bool isClue = false)
    {
        _id = id;
        _name = name;
        _icon = icon;
        _prefab = prefab;
        _isClue = isClue;
    }
}
