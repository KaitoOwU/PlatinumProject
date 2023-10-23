using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : ScriptableObject
{
    
    private string _name;
    [SerializeField] private int _id;
    private Sprite _icon;
    private GameObject _prefab;
    
    private bool _isThrowable;
    private GenerationZone _generationZones;
    
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

    public GenerationZone GenerationZoneAsEnum => _generationZones;

    public void SaveData(int id, string name, Sprite icon, GameObject prefab, bool isThrowable, GenerationZone generationZones)
    {
        _id = id;
        _name = name;
        _icon = icon;
        _prefab = prefab;
        _isThrowable = isThrowable;
        _generationZones = generationZones;
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
