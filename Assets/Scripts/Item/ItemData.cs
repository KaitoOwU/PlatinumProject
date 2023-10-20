using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : ScriptableObject
{
    
    [Header("--- DATAS ---")]
    [SerializeField] private string _name;
    [SerializeField] private int _id;
    [SerializeField] private Sprite _icon;
    [SerializeField] private GameObject _prefab;
    
    [Header("--- BEHAVIOUR ---")]
    [SerializeField] private bool _isThrowable;
    [SerializeField] private GenerationZone _generationZones;
    
    public string Name => _name;
    public int ID => _id;
    public Sprite Icon => _icon;
    public GameObject Prefab => _prefab;
    public bool IsThrowable => _isThrowable;

    public List<GenerationZone> GenerationZones
    {
        get
        {
            List<GenerationZone> generationZone = new();
            for(int i = 1; i <= 8; i *=2)
            {
                if((_generationZones & ((GenerationZone) i)) != 0)
                    generationZone.Add((GenerationZone) i);
            }

            return generationZone;
        }
    }

    public ItemData(int id, string name, Sprite icon, GameObject prefab)
    {
        _id = id;
        _name = name;
        _icon = icon;
        _prefab = prefab;
    }
}

[Flags]
public enum GenerationZone
{
    ZONE_1 = 1, //00000001
    ZONE_2 = 2, //00000010
    ZONE_3 = 4, //00000100
    ZONE_4 = 8  //00001000
}
