using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Murder Scenario", menuName = "Datas/Create Murder Scenario")]
public class MurderScenario : ScriptableObject
{

    [SerializeField] private SuspectDuo _suspectDuo;
    [SerializeField] private List<Clue> _clues;

    public SuspectDuo DuoSuspect => _suspectDuo;
    public List<Clue> Clues
    {
        get => _clues;
        set => _clues = value;
    }
    
    [Serializable]
    public struct SuspectDuo
    {
        [SerializeField] private SuspectData _victim, _murderer;
        public SuspectData Victim => _victim;
        public SuspectData Murderer => _murderer;

        public SuspectDuo(SuspectData victim, SuspectData murderer)
        {
            _victim = victim;
            _murderer = murderer;
        }

        public static bool operator ==(SuspectDuo a, SuspectDuo b) =>
            a.Victim == b.Victim && a.Murderer == b.Murderer;

        public static bool operator !=(SuspectDuo a, SuspectDuo b) =>
            a.Victim != b.Victim || a.Murderer != b.Murderer;
    }

    public void SaveData(SuspectDuo duo)
    {
        _suspectDuo = duo;
        _clues = new();
    }
}
