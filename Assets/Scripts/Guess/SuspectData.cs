using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "SuspectData", menuName = "Datas/Create Suspect Data", order = 2)]
public class SuspectData : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _image;


    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }
    public Sprite Image
    {
        get { return _image; }
        set { _image = value; }
    }

}
