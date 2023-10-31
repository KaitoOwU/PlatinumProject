using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueData : PickableData
{
    [SerializeField] protected MurderScenario.SuspectDuo _suspects;
    public MurderScenario.SuspectDuo Suspects => _suspects;

    public void SaveData(int id, string name, GameObject prefab, MurderScenario.SuspectDuo suspects)
    {
        _id = id;
        _name = name;
        _prefab = prefab;
        _suspects = suspects;
    }
}
