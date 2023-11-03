using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickableData : ScriptableObject
{
    
    [SerializeField] protected string _name;
    [SerializeField] protected int _id;
    [SerializeField] protected GameObject _prefab;
    
    public string Name => _name;
    public int ID => _id;
    public GameObject Prefab => _prefab;
}
