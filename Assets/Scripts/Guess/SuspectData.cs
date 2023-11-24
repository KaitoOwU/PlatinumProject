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
    [SerializeField, TextArea] private string _description; 


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
    
    public string Description => _description;

}
