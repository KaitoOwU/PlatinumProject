using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "GameData", menuName = "Datas/Create Game Constants", order = 1)]
public class GameData : ScriptableObject
{
    [Serializable]
    public struct TimerData
    {
        public int FirstPhaseTime => _firstPhaseTime;
        public int SecondPhaseTime => _secondPhaseTime;
        public int ThirdPhaseTime => _thirdPhaseTime;
        [SerializeField]
        private int _firstPhaseTime;
        [SerializeField]
        private int _secondPhaseTime;
        [SerializeField]
        private int _thirdPhaseTime;
    }

    [SerializeField] private TimerData _timerValues;
    [SerializeField] private SuspectData[] suspectsDatas;


    public TimerData TimerValues
    {
        get { return _timerValues; }
        set { _timerValues = value; }
    }    
    public SuspectData[] SuspectsDatas
    {
        get { return suspectsDatas; }
        set { suspectsDatas = value; }
    }
    
}
