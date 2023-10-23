using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Game Data", menuName = "Datas/Create Game Constants", order = 1)]
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


    public TimerData TimerValues
    {
        get { return _timerValues; }
        set { _timerValues = value; }
    }
    
}
