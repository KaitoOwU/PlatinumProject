using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "PlayerConstants", menuName = "Datas/Create Player Constants", order = 2)]
public class PlayerConstants : ScriptableObject
{
    public float NormalMoveSpeed
    {
        get =>_normalMoveSpeed; 
        set { _normalMoveSpeed = value; }
    }
    public float PushMoveSpeed
    {
        get => _pushMoveSpeed;
        set { _pushMoveSpeed = value; }
    }

    [SerializeField]
    float _normalMoveSpeed;
    [SerializeField]
    float _pushMoveSpeed;
}
