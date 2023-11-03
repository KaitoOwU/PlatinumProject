using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : PickableData
{
    [SerializeField] protected Sprite _uiIcon;
    public Sprite UIIcon => _uiIcon;

    public void SaveData(int id, string name, GameObject prefab, Sprite icon)
    {
        _name = name;
        _id = id;
        _prefab = prefab;
        _uiIcon = icon;
    }
}
