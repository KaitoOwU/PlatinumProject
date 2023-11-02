using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField] private SuspectData[] _suspectsDatas;
    [SerializeField] private int _furnitureCluesCount;

    public TimerData TimerValues => _timerValues;
    public SuspectData[] SuspectsDatas => _suspectsDatas;
    public int FurnitureCluesCount => _furnitureCluesCount;

}
